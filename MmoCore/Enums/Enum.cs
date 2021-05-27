using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmoCore.Enums
{
    public enum CONTENT_TYPE : ushort
    {
        NONE,
        TEST,
        HB_CHECK,
        RMC,    //remote method call?
        END,
    }
}
