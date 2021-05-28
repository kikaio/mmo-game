using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common.Networking;
using common.Protocols;
using common.Utils.Loggers;
using MmoCore.Packets;
using static TestClient.Tester;

namespace TestClient.State
{
    public class WaitWelcome : ClientState
    {
        public CoreLogger logger { get; set; } = new Log4Logger();
        public TesterSession session { get; set; }

        public WaitWelcome(CoreLogger _logger, TesterSession _session)
        {
            logger = _logger;
            session = _session;
        }

        public async Task AnalizerAsync_Ans(CoreSession _s, Packet _p)
        {
            var mmoPacket = new MmoCorePacket(_p);
            switch (mmoPacket.cType)
            {
                case MmoCore.Enums.CONTENT_TYPE.WELCOME:
                    {
                        var wp = new WelcomePacket(mmoPacket);
                        wp.PacketRead();
                        if (wp.sId < 0)
                        {
                            //todo : expired session?
                        }
                        else
                        {
                            logger.WriteDebug($"Tester Client Recv welcome ans, my session id is {wp.sId}");
                            session.SetSessionId(wp.sId);
                        }
                    }
                    break;
                case MmoCore.Enums.CONTENT_TYPE.RMC:
                    break;
                default:
                    break;
            }
        }

        public Task AnalizerAsync_Noti(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        public Task AnalizerAsync_Req(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        public Task AnalizerAsync_Test(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }
    }
}
