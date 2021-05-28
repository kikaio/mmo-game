using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using common.Utils.Loggers;
using MmoCore.Packets;
using MmoCore.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestClient
{
    public class Tester : CoreNetwork
    {
        public string host { get; private set; } = "127.0.0.1";
        public int port { get; private set; } = 30000;

        private Dictionary<string, Worker> workerMap = new Dictionary<string, Worker>();
        private Log4Logger logger = new Log4Logger();

        private CancellationTokenSource cts = new CancellationTokenSource();

        private TesterSession tSession = default(TesterSession);

        public Tester()
        {
        }

        private void ReadyWorker()
        {
            workerMap["input"] = new Worker("input");
            workerMap["receiver"] = new Worker("receiver", true);
            workerMap["dispatcher"] = new Worker("dispatcher", true);
            workerMap["hb"] = new Worker("hb", true);

            workerMap["input"].PushJob(new JobOnce(DateTime.UtcNow, () => {
                while (cts.IsCancellationRequested == false)
                {
                    string cmds = Console.ReadLine();
                    if (cmds.ToLower() == "exit")
                    {
                        cts.Cancel();
                        continue;
                    }
                    else
                    {
                        ChatNoti cNoti = new ChatNoti();
                        cNoti.msg = cmds;
                    }
                }
            }));

            workerMap["receiver"].PushJob(new JobOnce(DateTime.UtcNow, async () =>
            {
                //do sync recv
                while(cts.IsCancellationRequested == false)
                {
                    if (tSession.Sock.Sock.Connected == false)
                        break;
                    Packet p = await tSession.OnRecvTAP();
                    var newPackage = new Package(tSession, p);
                    packageQ.Push(newPackage);
                }
            }));

            workerMap["hb"].PushJob(new JobOnce(DateTime.UtcNow, async () =>
            {
                while (cts.IsCancellationRequested == false)
                {
                    if (tSession.Sock.Sock.Connected == false)
                        break;
                    var hb = new HBNoti();
                    hb.PacketWrite();
                    await tSession.OnSendTAP(hb.packet);
                    Thread.Sleep((int)(CoreSession.hbDelayMilliSec*0.75f));
                }
            }));

            workerMap["dispatcher"].PushJob(new JobOnce(DateTime.UtcNow, async () =>
            {
                while (cts.IsCancellationRequested)
                {
                    packageQ.Swap();
                    while (true)
                    {
                        var pkg = packageQ.pop();
                        if (pkg == null)
                            break;
                        await PackageDispatcherAsync(pkg);
                    }
                    await Task.Delay(100);
                }
            }));
        }

        private void ReadyTranslate()
        {
            MmoTranslate.Init();
        }

        public override void ReadyToStart()
        {
            ReadyTranslate();
            ReadyWorker();
        }

        private void Connect()
        {
            CoreTCP tcp = new CoreTCP(AddressFamily.InterNetwork);
            tcp.Sock.Connect(host, port);
            tSession = new TesterSession(-1, tcp);
        }

        public override void Start()
        {
            //after connect complete, worker start
            int tryCnt = 0;
            while (true)
            {
                try
                {
                    if (++tryCnt == 10)
                        break;
                    logger.WriteDebug($"try connect to server.....{tryCnt}");
                    Connect();
                    logger.WriteDebug("connect complete");
                    break;
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                    throw e;
                }
            }

            if (tryCnt == 10)
            {
                logger.Error("check server state");
                return;
            }

            foreach (var ele in workerMap)
            {
                logger.WriteDebug($"{ele.Key} is start");
                ele.Value.WorkStart();
            }

            //send hello packet
            Task.Factory.StartNew(async () => {
                logger.WriteDebug("start send hello packet to server");
                var hello = new HelloPacket();
                hello.packet.PacketWrite();
                if(await tSession.OnSendTAP(hello.packet))
                    logger.WriteDebug("send complete");
            });

        }
        #region SYNC
        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region ASYNC
        protected override async Task AnalizerAsync_Ans(CoreSession _s, Packet _p)
        {
            var mmoPacket = new MmoCorePacket(_p);
            switch (mmoPacket.cType)
            {
                case MmoCore.Enums.CONTENT_TYPE.WELCOME:
                    {
                        var wp = new WelcomePacket(mmoPacket);
                        wp.PacketRead();
                        if (wp.sId < 0)
                        {
                            //todo : expired session?
                        }
                        else
                        {
                            logger.WriteDebug($"Tester Client Recv welcome ans, my session id is {wp.sId}");
                            tSession.SetSessionId(wp.sId);
                        }
                    }
                    break;
                case MmoCore.Enums.CONTENT_TYPE.RMC:
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
