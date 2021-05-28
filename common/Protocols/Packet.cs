using common.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Protocols
{

    public interface ISerializePacket
    {
        void SerRead();
        void SerWrite();
    }

    public class Packet : ISerializePacket
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

        public NetStream header { get; protected set; } = new NetStream(GetHeaderSize());
        public NetStream data { get; protected set; }
        public PACKET_TYPE pType { get; protected set; } = PACKET_TYPE.NONE;

        public Packet(long _capacity = 512)
        {
            if (_capacity <= 0)
                data = null;
            else
                data = new NetStream(_capacity);
        }

        protected Packet(Packet _p)
        {
            header = _p.header;
            data = _p.data;
            pType = _p.pType;
        }

        public Packet(NetStream _h, NetStream _d)
        {
            header = _h;
            data = _d;
        }

        public void SetHeader(Int32 _val)
        {
            header.ResetOffset();
            header.WriteInt32(_val);
        }

        public void UpdateHeader()
        {
            if (data == null)
                SetHeader(0);
            else
                SetHeader(data.offset);
        }

        public Int32 GetHeader()
        {
            header.ResetOffset();
            return header.ReadInt32();
        }

        protected void ClearData()
        {
            if (data == null)
                return;
            Array.Clear(data.bytes, 0, data.bytes.Length);
            data.ResetOffset();
        }

        [Conditional("DEBUG")]
        public void RenderPacket(string _header = "")
        {
            Console.WriteLine($"Pacekt========{_header}");
            header?.RenderBytes();
            data?.RenderBytes();
            Console.WriteLine($"================");
        }
        [Conditional("DEBUG")]
        public void RenderProperties()
        {
            StringBuilder sb = new StringBuilder("[RenderProperties]");
            var pList = GetType().GetProperties();
            foreach (var pInfo in pList)
            {
                if (pInfo.CanWrite == false)
                    continue;
                sb.AppendLine($"{pInfo.Name}:{pInfo.GetValue(this).ToString()}");
            }
            Console.WriteLine(sb.ToString());
        }

        public void ReadPacketType()
        {
            pType = (PACKET_TYPE)data.ReadUInt16();
        }

        public void SerRead()
        {
            pType = (PACKET_TYPE)Translate.Read<ushort>(data);
        }

        public void SerWrite()
        {
            Translate.Write(data, (ushort)pType);
        }


        public virtual void PacketRead()
        {
            this.SerRead();
        }
        public virtual void PacketWrite()
        {
            this.SerWrite();
        }
    }
}
