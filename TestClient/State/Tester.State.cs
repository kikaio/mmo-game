using common.Networking;
using common.Protocols;
using common.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestClient.State;

namespace TestClient
{
    public partial class Tester
    {
        enum STATE
        {
            NONE,
            WAIT_CONN,
            WAIT_WELCOME,
            BEFORE_LOGIN,
            LOBBY_SERVER,
            BATTLE_SERVER,
            BATTLE_ROOM,
        }


        public interface ClientState
        {
            CoreLogger logger { get; set; }
            TesterSession session { get; set; }
            Task AnalizerAsync_Req(CoreSession _s, Packet _p);
            Task AnalizerAsync_Ans(CoreSession _s, Packet _p);
            Task AnalizerAsync_Noti(CoreSession _s, Packet _p);
            Task AnalizerAsync_Test(CoreSession _s, Packet _p);
        }

        public class ClientNonState : ClientState
        {
            public CoreLogger logger { get; set; } = new Log4Logger();
            public TesterSession session { get; set; }

            public ClientNonState(CoreLogger _l, TesterSession _s)
            {
                logger = _l;
                session = _s;
            }

            public Task AnalizerAsync_Ans(CoreSession _s, Packet _p)
            {
                return null;
            }

            public Task AnalizerAsync_Noti(CoreSession _s, Packet _p)
            {
                return null;
            }

            public Task AnalizerAsync_Req(CoreSession _s, Packet _p)
            {
                return null;
            }

            public Task AnalizerAsync_Test(CoreSession _s, Packet _p)
            {
                return null;
            }
        }


        STATE curState = STATE.NONE;
        Dictionary<STATE, ClientState> stateDict = new Dictionary<STATE, ClientState>();

        private void ReadyState()
        {
            stateDict[STATE.NONE] = new ClientNonState(logger, tSession);
            stateDict[STATE.WAIT_CONN] = new WaitConn(logger, tSession);
            stateDict[STATE.WAIT_WELCOME] = new WaitWelcome(logger, tSession);
        }

        private void SetClientState(STATE _s)
        {
            var preState = curState;
            if (_s != preState)
            {
                logger.WriteDebug($"client state change {_s} from {preState}");
            }
            curState = _s;
        }
    }
}
