using common.Networking;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Samples
{
    using common.Protocols;
    using System.Net;
    using System.Threading;
    using SessionList = List<CoreSession>;
    public class SampleServer : CoreNetwork
    {
        protected CoreSock listenSock;
        public SessionList expiredSessioinList = new SessionList();
        int backlogCnt = 100;
        CancellationToken isDownToken = new CancellationToken();
        public SampleServer(string _name, int _port, Action _shutdwn = null)
            : base(_name)
        {
            name = _name;
            shutdownAct = _shutdwn;
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _port);
        }

        protected virtual void BindAndListen()
        {
            listenSock.Sock.Bind(ep);
            listenSock.Sock.Listen(backlogCnt);
        }

        public override void ReadyToStart()
        {
            BindAndListen();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected void CheckHeartBeats()
        {
            if (isDownToken.IsCancellationRequested)
                return;
            foreach (var _s in SessionMgr.Inst.ToSessonList())
            {
                if (_s.HeartBeat < DateTime.UtcNow)
                    expiredSessioinList.Add(_s);
            }

            foreach (var _s in expiredSessioinList)
            {
                var removed = default(CoreSession);
                if(SessionMgr.Inst.CloseSession(_s.SessionId, out removed))
                    logger.WriteDebug($"session[{removed.SessionId}]'s heartbeat is expired and removed");
            }

            expiredSessioinList.Clear();
        }
    }
}
