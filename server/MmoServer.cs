using common.Networking;
using common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    public class MmoServer : CoreNetwork
    {
        //음.....우선 단일 로비홀 -> 룸 으로 할까.
        public override void ReadyToStart()
        {
            throw new NotImplementedException();
        }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Ans(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Noti(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Req(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }

        protected override void Analizer_Test(CoreSession _s, Packet _p)
        {
            throw new NotImplementedException();
        }
    }
}
