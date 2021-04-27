using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Protocols
{
    public class Packet
    {
        public static int GetHeaderSize()
        {
            return sizeof(Int32);
        }

        public enum PACKET_TYPE : ushort
        {
            NONE,
            NOTI,
            REQ,
            ANS,
            TEST,
            END,
        }



    }
}
