using common.Protocols;
using MmoCore.Packets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    public InputField MyIpTxt;
    public InputField MyPortTxt;

    public InputField ConnToIpTxt;
    public InputField ConnToPortTxt;

    public LobbyLog lobbyLog;

    private CancellationTokenSource cst = new CancellationTokenSource();
    private string serverProcessName = "server.exe";
    private string folderName = "Debug";

    private bool isWelcomed = false;

    private Networker lobbyNetworker;
    private AsyncOperation selectSceneOp;
#if TESTING
    bool isTest = true;
#else
    bool isTest = false;
#endif

    public string MyIp {
        get
        {
            return MyIpTxt?.text ?? string.Empty;
        }
    }

    public int MyPort { get {
            int ret = default(int);
            if (int.TryParse(MyPortTxt.text, out ret) == false)
                return -1;
            return ret;
        } }

    public void ClickConnectBtn()
    {
        //todo : connect to other player's server
        if(NetClient.Inst.PublicPort <= 0)
            return;
        if (string.IsNullOrWhiteSpace(NetClient.Inst.PublicIp))
            return;
        UnityEngine.Debug.Log($"ClickConnectBtn");
        lobbyLog.TryConnToLobbyServer();
        Task.Factory.StartNew(async () => 
        {
            UnityEngine.Debug.Log($"Conn Task is Started");
            lobbyNetworker = Networker.CreateNetworker("Lobby", () =>
            {
                UnityEngine.Debug.Log("LobbyNetworker shutdowned");
            });
            lobbyNetworker.ReadyToStart();
            lobbyNetworker.SetConnToServer(NetClient.Inst.PublicIp
                , NetClient.Inst.PublicPort);
            lobbyNetworker.StartConnect(()=> 
            {
                lobbyLog.ConnToLobyyComplete();
                //start send hello packet until recv welcome packet
                StartCoroutine(SendHelloPacket());
            });
        });
    }

    // send hello packet to lobbyserver while recv welcome packet.
    private IEnumerator SendHelloPacket()
    {
        float deltaSec = 0.3f;
        while (isWelcomed == false)
        {
            var hello = new HelloPacket();
            hello.PacketWrite();
            lobbyNetworker.mSession.OnSendTAP(hello.packet);
            yield return new WaitForSeconds(deltaSec);
        }
    }


    private void ChangeToSelectScene()
    {

        string sceneName = "SelectScene";
        SceneManager.LoadScene(sceneName);

        return;
        //change scene to select scene
        if (selectSceneOp.isDone)
            SceneManager.LoadScene(sceneName);
        else
        {
            Task.Factory.StartNew(async () => {
                while (selectSceneOp.isDone == false)
                    await Task.Delay(1000);
                SceneManager.LoadScene(sceneName);
            });
        }
    }


    private IEnumerator GetPublicHostName()
    {
        UnityEngine.Debug.Log($"Start GetPublicHostName");
        var ipEntries = Dns.GetHostEntry(Dns.GetHostName());

        string privateIp = string.Empty;
        foreach (var ele in ipEntries.AddressList)
        {
            if (ele.AddressFamily == AddressFamily.InterNetworkV6)
                continue;
            privateIp = ele.ToString();
            break;
        }
        UnityEngine.Debug.Log($"private : {privateIp}");

        string publicIp = privateIp;
        string otherWeb = "http://ipinfo.io/ip";
        using (var wc = new WebClient())
        {
            var ipStr = wc.DownloadString(otherWeb);
            if (string.IsNullOrWhiteSpace(ipStr.Trim()) == false)
                publicIp = ipStr;
        }

        //public , private 둘다?
        int ServerPort = 30000;
        //todo : get my public ip, port
        NetClient.Init(publicIp, ServerPort, privateIp, ServerPort);

        MyIpTxt.text = publicIp;
        MyPortTxt.text = $"{ServerPort.ToString()}";

        UnityEngine.Debug.Log($"public addr : {publicIp}/{ServerPort}");
        yield return null;
    }

    public void ClickRunBtn()
    {
        //server program name

        Task.Factory.StartNew(async () =>
        {
            lobbyLog.TryStartLobbyServer();
            
            
#if TESTING == false
        folderName = "Release";
#endif
            string serverExePath = $@".\ServerExe\{folderName}";
            await FileUtils.RunServerProcess(serverExePath, serverProcessName, MyPort);

            lobbyLog.ServerStartComplete();

        }, TaskCreationOptions.DenyChildAttach);
        
        //port forwarding을 위한 shell script
        string shellName = "";
        //현재 port는 30000번으로 고정된 상태.
        //Shell 실행 후 port forwarding, server process 실행.
        //FileUtils.RunShell(shellName, true);

        //todo : try connect to my server
    }

    public void Start()
    {
        StartCoroutine(GetPublicHostName());
        //StartCoroutine(AsyncLoadSelectScene());
    }

    private IEnumerator AsyncLoadSelectScene()
    {
        string sceneName = "SelectScene";
        selectSceneOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        selectSceneOp.allowSceneActivation = false;
        while (selectSceneOp.isDone == false)
            yield return null;
        UnityEngine.Debug.Log($"Scene Load Complete :{sceneName}");
    }

    public void Update()
    {
        if (lobbyNetworker == null)
            return;
        var pkg = lobbyNetworker.packageQ.pop();
        if (pkg == null)
        {
            lobbyNetworker.packageQ.Swap();
            return;
        }
        PkgDispatcher(pkg);
    }

    private void PkgDispatcher(Package _pkg)
    {
        var packet = _pkg.packet;
        var session = _pkg.session;

        packet.ReadPacketType();
        MmoCorePacket mp = new MmoCorePacket(packet);
        switch (mp.cType)
        {
            case MmoCore.Enums.CONTENT_TYPE.TEST:
                break;
            case MmoCore.Enums.CONTENT_TYPE.WELCOME:
                {
                    UnityEngine.Debug.Log("recv welcome packet");
                    //todo : scene change
                    isWelcomed = true;
                    ChangeToSelectScene();
                }
                break;
            default:
                break;
        }
    }
}
