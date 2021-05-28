using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using common.Utils.Loggers;
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
            logger = new Log4Logger();
            logger.WriteDebug("Server Start");
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

                Console.Write("Server is down");
                Console.ReadKey();
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
            pkgWorker.PushJob(new JobOnce(DateTime.UtcNow, async () =>
            {
                while (shutdownTokenSource.IsCancellationRequested == false)
                {
                    packageQ.Swap();
                    while (true)
                    {
                        var pkg = packageQ.pop();
                        if (pkg == default(Package))
                            break;
                        await PackageDispatcherAsync(pkg);
                    }
                    await Task.Delay(100);
                }
            }));

            var hpCheckWorker = mWorkerDict["hb"] = new Worker("hb");
            
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

            var cmdWorker = mWorkerDict["cmd"] = new Worker("cmd");
            cmdWorker.PushJob(new JobOnce(DateTime.UtcNow, () =>
            {
                while (shutdownTokenSource.IsCancellationRequested == false)
                {
                    string inputs = Console.ReadLine().ToUpper();
                    string[] cmds = inputs.Split(' ');
                    if (cmds.Length < 1)
                        return;
                    switch (cmds[0])
                    {
                        case "EXIT":
                            shutdownAct?.Invoke();
                            break;
                    }
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
            Task.Factory.StartNew(async ()=> {
                while (shutdownTokenSource.IsCancellationRequested == false)
                {
                    var sock = mListener.Sock.Accept();
                    var newSid = SessionMgr.Inst.GetNextSessionId();
                    CoreSession session = new CoreSession(newSid, new CoreTCP(sock));
                    {
                        //todo : new session created
                    }
                    SessionMgr.Inst.AddSession(session);
                    await Task.Factory.StartNew(async () =>
                    {
                        while (shutdownTokenSource.IsCancellationRequested == false
                        && session.Sock.Sock.Connected)
                        {
                            Packet p = await session.OnRecvTAP(() => {
                                //todo : session closed action
                            });
                            packageQ.Push(new Package(session, p));
                        }
                    }, TaskCreationOptions.DenyChildAttach);
                }
            });

            foreach (var ele in mWorkerDict)
            {
                logger.WriteDebug($"{ele.Key} work is start");
                ele.Value.WorkStart();
            }
        }

        public bool IsShutdownRequested()
        {
            return shutdownTokenSource.IsCancellationRequested;
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
                case CONTENT_TYPE.RMC:
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
                case CONTENT_TYPE.HELLO:
                    {
                        logger.WriteDebug($"recv hello packet from {_s.SessionId}");
                        //send welcome, contain session id
                        WelcomePacket wc = new WelcomePacket();
                        wc.sId = _s.SessionId;
                        wc.PacketWrite();
                        await _s.OnSendTAP(wc.packet);
                    }
                    break;
                case CONTENT_TYPE.RMC:
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
