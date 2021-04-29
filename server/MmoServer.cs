using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    public class MmoServer : CoreNetwork
    {
        CoreSock mListener = new CoreTCP(AddressFamily.InterNetwork);

        Dictionary<string, Worker> mWorkerDict = new Dictionary<string, Worker>();
        CancellationTokenSource shutdownTokenSource = new CancellationTokenSource();

        public MmoServer() : base("MMO", "", 30000)
        {
            ep = new IPEndPoint(IPAddress.Any, port);
            shutdownAct = () => {
                logger.WriteDebugWarn("Server shutdown called");
                shutdownTokenSource.Cancel();
                foreach (var _w in mWorkerDict.Values.ToList())
                {
                    logger.WriteDebugWarn($"[worker{_w.workerName}] work fin requested");
                    _w.WorkFinish();
                }
                mWorkerDict.Clear();
                int delaySec = 30;
                logger.WriteDebugWarn($"server down after {delaySec} Sec");
                while (delaySec > 0)
                {
                    logger.WriteDebugWarn($"{delaySec}");
                    Thread.Sleep(1000 * 1);
                    delaySec--;
                }
            };
        }

        private void BindAndListen()
        {
            mListener.Sock.Bind(ep);
            mListener.Sock.Listen(100);
        }
        private void ReadyWorkers()
        {
            var pkgWorker = mWorkerDict["pkg"] = new Worker("pkg");
            pkgWorker.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, 1000, () =>
            {
                while (shutdownTokenSource.Token.IsCancellationRequested == false)
                {
                    var pkg = packageQ.pop();
                    if (pkg == default(Package))
                        break;
                    PackageDispatcher(pkg);
                }
                packageQ.Swap();
            }));

            var hpCheckWorker = mWorkerDict["hb"] = new Worker("hb");
            hpCheckWorker.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, 1000, () =>
            {
                if (shutdownTokenSource.Token.IsCancellationRequested)
                    return;

                var delList = new List<CoreSession>();
                foreach (var s in SessionMgr.Inst.ToSessonList())
                {
                    if (s.HeartBeat < DateTime.UtcNow)
                        delList.Add(s);
                }

                foreach (var s in delList)
                {
                    var del = default(CoreSession);
                    if (SessionMgr.Inst.ForceCloseSession(s.SessionId, out del) == false)
                        logger.Error($"session[{s.SessionId}] force close is failed");
                }
            }));
        }
        //음.....우선 단일 로비홀 -> 룸 으로 할까.
        public override void ReadyToStart()
        {
            BindAndListen();
            ReadyWorkers();
        }

        public override void Start()
        {
            mWorkerDict["pkg"].WorkStart();
            mWorkerDict["hb"].WorkStart();
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
    }
}
