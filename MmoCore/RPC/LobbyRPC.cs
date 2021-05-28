using common.Networking;
using common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoCore.RPC
{
    [CoreRPC]
    public class LobbyRPC
    {
        //record hello to req stream
        [CoreRPCMethod(CoreRPCMgr.ECALL_TYPE.REQ)]
        public virtual void Greeting(NetStream _stream)
        {
            //record some key value etc..
            throw new NotImplementedException("Greeting_REQ");
        }

        //record hello to ans stream
        [CoreRPCMethod(CoreRPCMgr.ECALL_TYPE.ANS)]
        public virtual bool Greeting()
        {
            //check this session is fine?
            throw new NotImplementedException("Greeting_ANS");
        }

        [CoreRPCMethod(CoreRPCMgr.ECALL_TYPE.NOTI)]
        public virtual void SendHeartBeat(NetStream _stream)
        {
            //send packet hb
            throw new NotImplementedException("SendHeartBeat_NOTI");
        }
        [CoreRPCMethod(CoreRPCMgr.ECALL_TYPE.TEST)]
        public virtual void Test(NetStream _stream)
        {
            //lobby test somethings.
            throw new NotImplementedException("Test_TEST");
        }
    }
}
