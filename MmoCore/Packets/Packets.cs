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

        public MmoCorePacket(PACKET_TYPE _pt, CONTENT_TYPE _ct)
            : base(128)
        {
            pType = _pt;
            cType = _ct;
        }

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
    //clinet to server
    public class HelloPacket : MmoCorePacket
    {
        public HelloPacket() : base(PACKET_TYPE.REQ, CONTENT_TYPE.HELLO)
        {

        }
        public override void PacketRead()
        {
            base.PacketRead();
        }

        public override void PacketWrite()
        {
            base.PacketWrite();
        }
    }

    public class WelcomePacket : MmoCorePacket
    {
        public long sId { get; set; }

        public WelcomePacket() : base(PACKET_TYPE.ANS, CONTENT_TYPE.WELCOME)
        {
        }

        public WelcomePacket(MmoCorePacket _p) : base(_p)
        {
            var p = _p as WelcomePacket;
            if (p != null)
                cType = p.cType;
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

    public class ChatNoti : MmoCorePacket
    {
        public string msg;

        public ChatNoti() : base(PACKET_TYPE.NOTI, CONTENT_TYPE.CHAT)
        { }

        public ChatNoti(Packet _p) : base(_p)
        {
            var p = _p as ChatNoti;
            if (p != null)
                cType = p.cType;
        }

        public override void PacketRead()
        {
            base.PacketRead();
            msg = Translate.Read<string>(data);
        }

        public override void PacketWrite()
        {
            base.PacketWrite();
            Translate.Write(data, msg);
        }
    }
}
