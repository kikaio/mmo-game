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
            ReadContentType();
        }

        private void ReadContentType()
        {
            cType = Translate.Read<CONTENT_TYPE>(data);
        }

        public override void PacketRead()
        {
        }

        public override void PacketWrite()
        {
            base.PacketWrite();
            Translate.Write(data, (ushort)cType);
        }
    }

    public class HBNoti 
    {
        public MmoCorePacket packet { get; private set; }
        public HBNoti()
        {
            packet = new MmoCorePacket(Packet.PACKET_TYPE.NOTI, CONTENT_TYPE.HB_CHECK);
        }
        public HBNoti(MmoCorePacket _packet)
        {
            packet = _packet;
        }

        public void PacketRead()
        {
        }

        public void PacketWrite()
        {
            packet.PacketWrite();
            packet.UpdateHeader();
        }
    }


    //clinet to server
    public class HelloPacket 
    {
        public MmoCorePacket packet { get; set; }
        public HelloPacket() 
        {
            packet = new MmoCorePacket(Packet.PACKET_TYPE.REQ, CONTENT_TYPE.HELLO);
        }
        public HelloPacket(MmoCorePacket _p)
        {
            packet = _p;
        }
        public void PacketRead()
        {
        }

        public void PacketWrite()
        {
            packet.PacketWrite();
            packet.UpdateHeader();
        }
    }

    public class WelcomePacket
    {
        public MmoCorePacket packet { get; set; }
        public long sId { get; set; }

        public WelcomePacket() 
        {
            packet = new MmoCorePacket(Packet.PACKET_TYPE.ANS, CONTENT_TYPE.WELCOME);
        }
        public WelcomePacket(MmoCorePacket _p)
        {
            packet = _p;
        }
        public void PacketRead()
        {
            sId = Translate.Read<long>(packet.data);
        }

        public void PacketWrite()
        {
            packet.PacketWrite();
            Translate.Write(packet.data, sId);
            packet.UpdateHeader();
        }
    }

    public class ChatNoti 
    {
        public MmoCorePacket packet { get; set; }

        public string msg;

        public ChatNoti()
        {
            packet = new MmoCorePacket(Packet.PACKET_TYPE.NOTI, CONTENT_TYPE.CHAT);
        }
        public ChatNoti(MmoCorePacket _p)
        {
            packet = _p;
        }
        public void PacketRead()
        {
            msg = Translate.Read<string>(packet.data);
        }

        public void PacketWrite()
        {
            packet.PacketWrite();
            Translate.Write(packet.data, msg);
            packet.UpdateHeader();
        }
    }
}
