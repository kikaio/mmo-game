using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using MmoCore.Enums;
using MmoCore.Packets;
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

        public MmoServer() : base("MMO", 30000)
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

                logger.WriteDebugWarn($"call CloseAllSession");
                SessionMgr.Inst.CloseAllSession();

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
            var pkgWorker = mWorkerDict["pkg"] = new Worker("pkg", true);
            pkgWorker.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, 1000, () =>
            {
                while(shutdownTokenSource.IsCancellationRequested == false)
                {
                    var pkg = packageQ.pop();
                    if (pkg == default(Package))
                        break;
                    PackageDispatcher(pkg);
                }
                packageQ.Swap();
            }));

            var hpCheckWorker = mWorkerDict["hb"] = new Worker("hb", true);
            
            long hbCehckTicks = TimeSpan.FromMilliseconds(CoreSession.hbDelayMilliSec).Ticks;
            hpCheckWorker.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, hbCehckTicks, () =>
            {
                if (shutdownTokenSource.IsCancellationRequested)
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

            var cmdWorker = mWorkerDict["cmd"] = new Worker("cmd", true);
            cmdWorker.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, 1000, () =>
            {
                if (shutdownTokenSource.IsCancellationRequested)
                    return;
                string inputs = Console.ReadLine().ToUpper();
                string[] cmds = inputs.Split(' ');
                if (cmds.Length < 1)
                    return;
                switch (cmds[0])
                {
                    case "TEST":
                        {
                            //do something
                        }
                        break;
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
        #region SyncDispatch
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

        #region AsyncDispatch

        protected override async Task AnalizerAsync_Ans(CoreSession _s, Packet _p)
        {
            MmoCorePacket p = new MmoCorePacket(_p);
            switch (p.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebug($"[{_s.SessionId}] send Test Packet_Ans");
                    }
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
            return;
        }

        protected override async Task AnalizerAsync_Noti(CoreSession _s, Packet _p)
        {
            MmoCorePacket p = new MmoCorePacket(_p);
            switch (p.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebug($"[{_s.SessionId}] send Test Packet_Noti");
                    }
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    {
                        _s.UpdateHeartBeat();
                        logger.WriteDebug($"[{_s.SessionId}] hb update");
                    }
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
            return;
        }
        protected override async Task AnalizerAsync_Req(CoreSession _s, Packet _p)
        {
            MmoCorePacket p = new MmoCorePacket(_p);
            switch (p.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebug($"[{_s.SessionId}] send Test Packet_Req");
                    }
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
            return;
        }
        protected override async Task AnalizerAsync_Test(CoreSession _s, Packet _p)
        {
            MmoCorePacket p = new MmoCorePacket(_p);
            switch (p.cType)
            {
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebug($"[{_s.SessionId}] send Test Packet_Test");
                    }
                    break;
                case CONTENT_TYPE.NONE:
                case CONTENT_TYPE.HB_CHECK:
                case CONTENT_TYPE.END:
                default:
                    break;
            }
            return;
        }
        #endregion
    }
}
