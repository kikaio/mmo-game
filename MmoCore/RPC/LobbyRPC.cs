using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoCore.RPC
{
    //일단 구현에 목표를?
    public interface LobbyRPC
    {
        Task Login_req(string _name, string _pw);
        Task<string> Login_ans();
    }
}
