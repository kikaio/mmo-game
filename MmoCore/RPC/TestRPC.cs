using common.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoCore.RPC
{
    [CoreRPC]
    public class TestRPC
    {
        [CoreRPCMethod(CoreRPCMgr.ECALL_TYPE.REQ)]
        public virtual void test(int _val)
        {
            throw new NotImplementedException("test");
        }
        [CoreRPCMethod(CoreRPCMgr.ECALL_TYPE.ANS)]
        public virtual bool test()
        {
            throw new NotImplementedException("test");
        }
    }
}
