using common.Networking;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class TesterSession : CoreSession
    {
        public TesterSession(long _sid, CoreSock _csock)
            : base(_sid, _csock)
        {
        }
    }
}
