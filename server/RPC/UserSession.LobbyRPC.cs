using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using common.Networking;
using MmoCore.RPC;

public partial class UserSession : LobbyRPC
{
    public override bool Greeting()
    {
        return false;
    }
}
