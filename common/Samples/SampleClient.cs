using common.Networking;
using common.Protocols;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace common.Samples
{
    //Sync, Tcp 기반 샘플용 client
    public class SampleClient : CoreNetwork
    {
        private CoreSession mSession = default(CoreSession);

        public SampleClient(string _host, int _port)
        {
            ep = new IPEndPoint(IPAddress.Parse(_host), _port);
        }
        public override void ReadyToStart()
        {
            throw new NotImplementedException();
        }

        private void ConnectToServer()
        {
            var sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(ep);
            mSession = new CoreSession(-1, new CoreTCP(sock));
        }

        public override void Start()
        {
            ConnectToServer();
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    break;
                case CONTENT_TYPE.TEST:
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    break;
                case CONTENT_TYPE.TEST:
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    break;
                case CONTENT_TYPE.TEST:
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            var sp = new SamplePacket(_p);
            switch (sp.cType)
            {
                case CONTENT_TYPE.NONE:
                    break;
                case CONTENT_TYPE.HB_CHECK:
                    break;
                case CONTENT_TYPE.TEST:
                    break;
                case CONTENT_TYPE.END:
                    break;
                default:
                    break;
            }
        }
    }
}
