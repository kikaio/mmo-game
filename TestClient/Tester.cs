using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using common.Utils.Loggers;
using MmoCore.Packets;
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

        private Dictionary<string, Worker> workerMap;
        private Log4Logger logger = new Log4Logger();

        private CancellationTokenSource cts = new CancellationTokenSource();

        private TesterSession tSession = default(TesterSession);

        public Tester()
        {
        }

        private void ReadyWorker()
        {
            workerMap["input"] = new Worker("input");
            workerMap["receiver"] = new Worker("receiver");
            workerMap["dispatcher"] = new Worker("dispatcher");
            workerMap["hb"] = new Worker("hb");

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

            workerMap["receiver"].PushJob(new JobNormal(DateTime.UtcNow, DateTime.MaxValue, 100, () =>
            {
                //do sync recv
                while(cts.IsCancellationRequested == false)
                {
                    HelloPacket p = new HelloPacket();
                    SendPacketSync(p);
                }
            }));

        }

        public override void ReadyToStart()
        {
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
            try
            {
                logger.WriteDebug("Tester Connect Start");
                Connect();
                logger.WriteDebug("Tester Connect Complete");

                foreach (var ele in workerMap)
                {
                    logger.WriteDebug($"{ele.Key} is start");
                    ele.Value.WorkStart();
                }

                //send hello packet

            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                throw e;
            }
        }

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

        private void SendPacketSync(Packet _p)
        {
            _p.UpdateHeader();
            int remainCnt = _p.header.bytes.Length;
            while (remainCnt > 0)
            {
                int offset = _p.header.bytes.Length - remainCnt;
                remainCnt -= tSession.Sock.Sock.Send(_p.header.bytes, offset, remainCnt, SocketFlags.None);
            }
            remainCnt = _p.GetHeader();
            while (remainCnt > 0)
            {
                int offset = _p.header.bytes.Length - remainCnt;
                remainCnt -= tSession.Sock.Sock.Send(_p.header.bytes, offset, remainCnt, SocketFlags.None);
            }
        }
    }
}
