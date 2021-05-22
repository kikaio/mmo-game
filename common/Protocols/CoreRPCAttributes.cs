using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common.Protocols
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CoreRPC : Attribute
    {
        public CoreRPCMgr.ECALL_TYPE mCallType;
        public string mMethodName;
        public Type objType;

        public CoreRPC(CoreRPCMgr.ECALL_TYPE _cType, string _mName, Type _objType)
        {
            mCallType = _cType;
            mMethodName = _mName;
            objType = _objType;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class CoreSerProperty : Attribute
    {
        public Type mType;
        public CoreSerProperty(Type _type)
        {
        }
    }
}
