using common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Protocols
{
    public class Package
    {
        public CoreSession session { get; private set; } = null;
        public Packet Packet { get; private set; } = null;

        public Package(CoreSession _s, Packet _p)
        {
            session = _s;
            Packet = _p;
        }
    }
}
