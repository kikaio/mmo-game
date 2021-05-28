using common.Protocols;
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
        public byte[] xorKeyBytes = Encoding.UTF8.GetBytes("testKey");
        private bool IsXorAble
        {
            //get { return false; }
            get { return xorKeyBytes != null; }
        }
        #endregion
        #region About DhEncrypt
        private byte[] dh_key = null;
        private byte[] dh_iv = null;
        private bool IsDhAble
        {
            get { return dh_key != null && dh_iv != null; }
        }

        public void SetDhInfo(byte[] _key, byte[] _iv)
        {
            dh_key = _key;
            dh_iv = _iv;
        }

        public async Task<bool> OnSendTAP(Packet _p, Action _closed = null)
        {
            int remainCnt = Packet.GetHeaderSize();
            int dataLength = _p.GetHeader();
            int offset = 0;
            while (remainCnt > 0)
            {
                offset = _p.header.bytes.Length - remainCnt;
                var sentCnt = Sock.Sock.Send(_p.header.bytes, offset, remainCnt, System.Net.Sockets.SocketFlags.None);
                if (sentCnt < 1)
                {
                    //todo : session closed
                    _closed?.Invoke();
                    return false;
                }
                remainCnt -= sentCnt;
            }

            remainCnt = dataLength;
            offset = 0;
            while (remainCnt > 0)
            {
                offset = dataLength - remainCnt;
                var sentCnt = Sock.Sock.Send(_p.data.bytes, offset, remainCnt, System.Net.Sockets.SocketFlags.None);
                if (sentCnt < 1)
                {
                    //todo : session closed
                    _closed?.Invoke();
                    return false;
                }
                remainCnt -= sentCnt;
            }
            return true;
        }

        public async Task<Packet> OnRecvTAP(Action _closed = null)
        {
            var ret = default(Packet);
            int offset = 0;
            int remainCnt = Packet.GetHeaderSize();
            NetStream header = new NetStream(remainCnt);
            while (remainCnt > 0)
            {
                offset = header.bytes.Length - remainCnt;
                int recvCnt = Sock.Sock.Receive(header.bytes, offset, remainCnt, System.Net.Sockets.SocketFlags.None);
                if (recvCnt < 1)
                {
                    //todo : session closed
                    _closed?.Invoke();
                    return default(Packet);
                }
                remainCnt -= recvCnt;
            }

            int dataLength = header.ReadInt32();
            offset = 0;
            remainCnt = dataLength;
            NetStream data = new NetStream(dataLength);
            while (remainCnt > 0)
            {
                offset = dataLength - remainCnt;
                int recvCvnt = Sock.Sock.Receive(data.bytes, offset, remainCnt, System.Net.Sockets.SocketFlags.None);
                if (recvCvnt < 1)
                {
                    //todo : session closed
                    _closed?.Invoke();
                    return default(Packet);
                }
                remainCnt -= recvCvnt;
            }

            ret = new Packet(header, data);
            return ret;
        }
        #endregion
    }
}
