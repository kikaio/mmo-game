using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server.Utils
{
    public class IPUtils
    {
        public struct NetInfo
        {
            public string public_Ip;
            public string private_Ip;

            public int public_port;
            public int private_port;
        }

        public static async Task<NetInfo> GetPublicIpStr()
        {
            var netInfo = new NetInfo();
            var ipEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
            using (var wc = new WebClient())
            {
                string checkWebAddr = "http://ipinfo.io/ip";
                var ipStr = await wc.DownloadStringTaskAsync(checkWebAddr);
                if (string.IsNullOrWhiteSpace(ipStr.Trim()) == false)
                {
                    netInfo.public_Ip = ipStr.Trim();
                    netInfo.private_Ip = "127.0.0.1";
                }
            }
            return netInfo;
        }
    }
}
