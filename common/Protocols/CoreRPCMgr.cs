using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Protocols
{
    //int : errorcode
    using RPCWrapper = Func<List<object>, int>;
    using RPCMap = Dictionary<string, RPCWrapper>;
    using RPCCallTypeToMap = Dictionary<CoreRPCMgr.ECALL_TYPE, RPCMap>;
    using EntityRPCMap = Dictionary<Type, RPCTypeToMap>;

    public class CoreRPCMgr
    {
        public enum ECALL_TYPE
        {
            REQ,
            ANS,
            NOTI,
            TEST,
        };

        private static CoreRPCMgr inst;
        public static CoreRPCMgr Inst
        {
            get {
                if (inst == null)
                    inst = new CoreRPCMgr();
                return inst;
            }
        }
    }

}
