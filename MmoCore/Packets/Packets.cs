using common.Protocols;
using MmoCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoCore.Packets
{
    public class MmoCorePacket : Packet
    {
        public CONTENT_TYPE cType { get; set; } = CONTENT_TYPE.NONE;

        public MmoCorePacket(Packet _p) : base(_p)
        {
            var ret = _p as MmoCorePacket;
            if (ret != default(MmoCorePacket))
                cType = ret.cType;
        }

        public override void PacketRead()
        {
            base.PacketRead();
            cType = (CONTENT_TYPE)Translate.Read<ushort>(data);
        }

        public override void PacketWrite()
        {
            base.PacketWrite();
            Translate.Write(data, (ushort)cType);
        }
    }



    public class WelcomePacket : MmoCorePacket
    {
        public long sId { get; set; }

        public WelcomePacket(MmoCorePacket _p) : base(_p)
        {
        }

        public override void PacketRead()
        {
            base.PacketRead();
            sId = Translate.Read<long>(data);
        }

        public override void PacketWrite()
        {
            base.PacketWrite();
            Translate.Write(data, sId);
        }
    }
}
