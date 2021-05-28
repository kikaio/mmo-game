using common.Networking;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.RPC
{
    public partial class UserSession : CoreSession
    {
        public UserSession(long _sid, CoreSock _sock) : base(_sid, _sock)
        {
        }
    }
}
