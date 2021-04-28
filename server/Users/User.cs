using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Users
{
    using UserId = Int64;
    public class User
    {
        public UserId uId { get; private set; } = 0;

        public static User Create(UserId _newId)
        {
            var ret = default(User);
            ret.uId = _newId;
            return ret;
        }

        [Conditional("DEBUG")]
        public void Render()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"User[{uId}]");
        }
    }
}
