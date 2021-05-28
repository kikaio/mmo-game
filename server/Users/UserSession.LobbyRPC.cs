using MmoCore.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Users
{
    public partial class UserSession : LobbyRPC
    {
        public Task<string> Login_ans()
        {
            throw new NotImplementedException();
        }

        public async Task Login_req(string _name, string _pw)
        {
            return;
        }
    }
}
