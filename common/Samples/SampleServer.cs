using common.Networking;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Samples
{
    using common.Jobs;
    using common.Protocols;
    using System.Net;
    using System.Threading;
    using SessionList = List<CoreSession>;
    public class SampleServer : CoreNetwork
    {
        //Sync, TCP Socket 기반 샘플 서버.
        protected CoreSock listenSock;
        public SessionList expiredSessioinList = new SessionList();
        int backlogCnt = 100;
        CancellationToken isDownToken = new CancellationToken();
        private Dictionary<string, Worker> wDict = new Dictionary<string, Worker>();


        public SampleServer(string _name, int _port)
            : base(_name)
        {
            name = _name;
            shutdownAct = SampleShutDown;
            ep = new IPEndPoint(IPAddress.Any, _port);
        }

        private void SampleShutDown()
        {
            logger.WriteDebugWarn("Server down req has called");
            SessionMgr.Inst.CloseAllSession();
        }

        protected virtual void BindAndListen()
        {
            listenSock.Sock.Bind(ep);
            listenSock.Sock.Listen(backlogCnt);
        }

        private void ReadyWorker()
        {
            var pkgW = wDict["for pkg"] = new Worker("for pkg");
            var hbW = wDict["for hb check"] = new Worker("for hb check");

            pkgW.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, 1000, ()=> {
                while (true)
                {
                    var pkg = this.packageQ.pop();
                    if (pkg == null)
                    {
                        packageQ.Swap();
                        break;
                    }
                    PackageDispatcher(pkg);
                } 
            }));
            
            hbW.PushJob(new JobNormal(DateTime.MinValue, DateTime.MaxValue, 1000, ()=> {
                var sList = SessionMgr.Inst.ToSessonList();
                var expiredList = new List<CoreSession>();
                foreach(var s in sList)
                {
                    if (s.HeartBeat < DateTime.UtcNow)
                        expiredList.Add(s);
                }

                foreach (var es in expiredList)
                {
                    var closedSession = default(CoreSession);
                    if (SessionMgr.Inst.ForceCloseSession(es.SessionId, out closedSession))
                    {
                        logger.WriteDebugWarn($"session is closed by hb check : {closedSession.SessionId}");
                    }
                }
            }));
        }

        //start전에 관련 준비는 여기서 모두 처리.
        public override void ReadyToStart()
        {
            logger.WriteDebugTrace();
            BindAndListen();
            ReadyWorker();
        }

        private void StartAccept()
        {
            logger.WriteDebugTrace();
            while (isDownToken.IsCancellationRequested == false)
            {
                var newSock = listenSock.Sock.Accept();
                long sId = SessionMgr.Inst.GetNextSessionId();
                var newSession = new CoreSession(sId, new CoreTCP(newSock));
                SessionMgr.Inst.AddSession(newSession);
                logger.WriteDebug($"welcome new client session - {sId}");
            }
        }

        public override void Start()
        {
            logger.WriteDebugTrace();
            Task.Factory.StartNew(StartAccept);
            Task.Factory.StartNew(CheckHeartBeats);
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebugTrace($"catch test packet from {_s.SessionId}");
                    }
                    break;
                default:
                    logger.WriteDebugTrace($"this contents type someting wrong-{sp.cType.ToString()}");
                    break;
            }
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebugTrace($"catch test packet from {_s.SessionId}");
                    }
                    break;
                default:
                    logger.WriteDebugTrace($"this contents type someting wrong-{sp.cType.ToString()}");
                    break;
            }
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.HB_CHECK:
                    {
                        _s.UpdateHeartBeat();
                        logger.WriteDebug($"catch hb noti from {_s.SessionId}");
                    }
                    break;
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebugTrace($"catch test packet from {_s.SessionId}");
                    }
                    break;
                default:
                    logger.WriteDebugTrace($"this contents type someting wrong-{sp.cType.ToString()}");
                    break;
            }
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.TEST:
                    {
                        logger.WriteDebugTrace($"catch test packet from {_s.SessionId}");
                    }
                    break;
                default:
                    logger.WriteDebugTrace($"this contents type someting wrong-{sp.cType.ToString()}");
                    break;
            }
        }

        protected void CheckHeartBeats()
        {
            while (isDownToken.IsCancellationRequested == false)
            {
                foreach (var _s in SessionMgr.Inst.ToSessonList())
                {
                    if (_s.HeartBeat < DateTime.UtcNow)
                        expiredSessioinList.Add(_s);
                }
                foreach (var _s in expiredSessioinList)
                {
                    var removed = default(CoreSession);
                    if (SessionMgr.Inst.CloseSession(_s.SessionId, out removed))
                        logger.WriteDebug($"session[{removed.SessionId}]'s heartbeat is expired and removed");
                }
                expiredSessioinList.Clear();
            }
        }
    }
}
