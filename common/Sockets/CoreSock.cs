using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace common.Sockets
{
    public abstract class CoreSock
    {
        public Socket Sock;
        public int Port;
        public IPAddress Addr;
    }

    public class CoreTCP : CoreSock
    {
        public CoreTCP(Socket _sock)
        {
            Sock = _sock;
        }

        public CoreTCP(AddressFamily _af = AddressFamily.InterNetwork)
        {
            Sock = new Socket(_af, SocketType.Stream, ProtocolType.Tcp);
        }
    }

    public class CoreUDP : CoreSock
    {
        public CoreUDP(AddressFamily _af = AddressFamily.InterNetwork)
        {
            Sock = new Socket(_af, SocketType.Dgram, ProtocolType.IP);
        }
    }
}
