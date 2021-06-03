using common.Jobs;
using common.Networking;
using common.Protocols;
using common.Sockets;
using common.Utils.Loggers;
using MmoCore.Enums;
using MmoCore.Packets;
using MmoCore.Protocols;
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

        public MmoServer(int _port = 30000) : base("MMO", _port)
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
                
                Console.Write("Server is down");
            };
        }

        private void ReadyTranslate()
        {
            MmoTranslate.Init();
        }

        private void BindAndListen()
        {
            mListener.Sock.Bind(ep);
            mListener.Sock.Listen(100);
        }
        
        private void ReadyWorkers()
        {
            mWorkerDict["pkg"] = new Worker("pkg", true);
            mWorkerDict["pkg"].PushJob(new JobOnce(DateTime.MinValue, async () =>
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

            mWorkerDict["hb"] = new Worker("hb");
            
            long hbCehckTicks = TimeSpan.FromMilliseconds(CoreSession.hbDelayMilliSec).Ticks;
            mWorkerDict["hb"].PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, hbCehckTicks, () =>
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

            mWorkerDict["cmd"] = new Worker("cmd");
            mWorkerDict["cmd"].PushJob(new JobOnce(DateTime.MinValue, () =>
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

        public override void ReadyToStart()
        {
            ReadyTranslate();
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
                    Task.Factory.StartNew(async () =>
                    {
                        while (shutdownTokenSource.IsCancellationRequested == false
                        && session.Sock.Sock.Connected)
                        {
                            Packet p = await session.OnRecvTAP(() => {
                                //todo : session closed action
                            });
                            if(p != default(Packet))
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
                        if (await _s.OnSendTAP(wc.packet))
                            logger.WriteDebug($"send welcome to {_s.SessionId}");
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
