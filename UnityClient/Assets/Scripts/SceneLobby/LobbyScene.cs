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
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    public InputField MyIpTxt;
    public InputField MyPortTxt;

    public InputField ConnToIpTxt;
    public InputField ConnToPortTxt;

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

    public string ConnToIp { get {
            return ConnToIpTxt?.text ?? string.Empty;
        } }
    public int ConnToPort { get {
            int ret = default(int);
            if (int.TryParse(ConnToPortTxt.text, out ret) == false)
                return -1;
            return ret;
        } }

    public void ClickConnectBtn()
    {
        UnityEngine.Debug.Log($"ClickConnectBtn");
        //todo : connect to other player's server
        if (ConnToPort < 0)
            return;
        if (ConnToIp == string.Empty)
            return;
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

    public void ClickCheckBtn()
    {
        StartCoroutine(GetPublicHostName());
    }

    public void ClickRunBtn()
    {
        //server program name

        Task.Factory.StartNew(async () =>
        {
            string psName = "server.exe";
            string folderName = "Debug";
#if TESTING == false
        folderName = "Release";
#endif
            string serverExePath = $@".\ServerExe\{folderName}";
            await FileUtils.RunServerProcess(serverExePath, psName, MyPort);
        }, TaskCreationOptions.DenyChildAttach);
        
        //port forwarding을 위한 shell script
        string shellName = "";
        //현재 port는 30000번으로 고정된 상태.
        //Shell 실행 후 port forwarding, server process 실행.
        //FileUtils.RunShell(shellName, true);

        //todo : try connect to my server
    }

}
