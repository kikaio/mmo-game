using common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Samples
{
    public class SamplePacket : Packet
    {
        public CONTENT_TYPE cType = CONTENT_TYPE.NONE;

        public SamplePacket(long _capacity = 512) : base(_capacity)
        {
        }

        public SamplePacket(Packet _p) : base(_p)
        {
            var sp = _p as SamplePacket;
            if (sp != null)
            {
                cType = sp.cType;
            }
        }
        
    }
}
