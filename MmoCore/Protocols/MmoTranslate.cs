using common.Networking;
using common.Protocols;
using MmoCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoCore.Protocols
{
    //must regist translate data for mmoservice
    public static class MmoTranslate
    {
        public static void Init()
        {
            Translate.RegistCustom<CONTENT_TYPE>((NetStream _s, object _val)
                => {
                    _s.WriteInt32((Int32)_val);
                }, 
            (NetStream _s)=> {
                var ret = default(CONTENT_TYPE);
                ret = (CONTENT_TYPE)_s.ReadInt32();
                return ret;
            });
        }
    }
}
