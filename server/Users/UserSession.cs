using common.Networking;
using common.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Users
{
    public partial class UserSession : CoreSession
    {
        public UserSession(long _sid, CoreSock _cs)
            : base(_sid, _cs)
        {
        }
    }
}
