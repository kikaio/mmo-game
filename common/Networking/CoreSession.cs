using common.Sockets;
using common.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Networking
{
    public class CoreSession : IDisposable
    {
        public long SessionId { get; protected set; } = 0;
        public CoreSock Sock { get; protected set; } = null;
        public DateTime HeartBeat { get; private set; } 
            = DateTime.UtcNow.AddMilliseconds(hbDelayMilliSec);

        private CoreLogger logger = new ConsoleLogger();
#if DEBUG
        public static int hbDelayMilliSec { get; private set; } = 20 * 1000;
#else 
        public static int hbDelayMilliSec { get; private set; } = 3 * 60 * 1000;
#endif
        protected bool IsDisposed = false;

        public CoreSession(long _sid, CoreSock _sock)
        {
            SessionId = _sid;
            Sock = _sock;
        }

        public void SetSessionId(long _sid)
        {
            SessionId = _sid;
        }

        public void UpdateHeartBeat()
        {
            HeartBeat = DateTime.UtcNow
                .AddMilliseconds(CoreSession.hbDelayMilliSec);
        }

        public bool IsExpireHeartBeat()
        {
            if (HeartBeat > DateTime.UtcNow)
                return false;
            return true;
        }

        public void DoDispose(bool _flag)
        {
            if (IsDisposed)
                return;
            IsDisposed = _flag;
            {
            }
        }

        public void Dispose()
        {
            DoDispose(true);
        }

        public void SetSocket(CoreSock _sock)
        {
            if (Sock.Sock.Connected)
                Sock.Sock.Close();
            Sock = _sock;
        }

        public void SetSyncSetting()
        {
            //socket 속성 설정.
            if (Sock is CoreTCP)
            {
                var curSocket = Sock.Sock;
                //Nagle algorithm 미적용.
                curSocket.NoDelay = true;
                //recv timeout 1초로 지정.
                var sec = 1 * 1000;
                curSocket.ReceiveTimeout = sec * 1;
            }
            else if (Sock is CoreUDP)
            {

            }
        }

        #region About Xor
        #endregion
        #region About DhEncrypt
        #endregion

    }
}
