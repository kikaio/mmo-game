using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common.Networking;
using common.Protocols;
using common.Utils.Loggers;
using static TestClient.Tester;

namespace TestClient.State
{
    public class WaitConn : ClientState
    {
        public CoreLogger logger { get; set; } = new Log4Logger();
        public TesterSession session { get; set; }

        public WaitConn(CoreLogger _logger, TesterSession _session)
        {
            logger = _logger;
            session = _session;
        }

        public Task AnalizerAsync_Ans(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        public Task AnalizerAsync_Noti(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        public Task AnalizerAsync_Req(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        public Task AnalizerAsync_RMC(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        public Task AnalizerAsync_Test(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }
    }
}
