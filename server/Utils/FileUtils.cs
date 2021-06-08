using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Utils
{
    public class FileUtils
    {
        public static async Task PortfowardBatchCreate(
            string _folderPath, string _batchFileName
            , string _publicIp, string _localIp
            , int _publicPort, int _localPort)
        {
            string batchPath = $"{_folderPath}/{_batchFileName}";
            using (var stream = File.Open(batchPath, FileMode.OpenOrCreate))
            {
                stream.SetLength(0);
            }

            using (var st = File.OpenWrite(batchPath))
            {
                using (var ws = new StreamWriter(st))
                {
                    /*
                    //batch cmd for portforwardiong and release
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("echo portfowarding start");
                    sb.Append($"netsh interface portproxy add v4tov4 ");
                    sb.Append($"listenport={_publicPort} listenaddress={_publicIp} ");
                    sb.AppendLine($"connectport={_localPort} connectaddress={_localIp}");
                    sb.AppendLine($"echo portfowarding finished");
                    sb.AppendLine($"echo if you try any input, the port will be release");
                    sb.AppendLine($"pause");
                    sb.Append($"netsh interface portproxy delete v4tov4 ");
                    sb.AppendLine($"listenport={_localPort} listenaddress= {_localIp}");
                    sb.AppendLine($"echo portforwarding is released. {_localIp}:{_localPort} is free");
                    sb.AppendLine($"echo press any key....");
                    sb.AppendLine($"pause");
                     */

                    //batch cmd for portforwardiong and release
                    StringBuilder sb = new StringBuilder();
                    string fireWallRuleName = "PortOpenTest";
                    //open firewall port 
                    sb.AppendLine(" echo portfowarding start");
                    sb.Append($" netsh advfirewall firewall add rule name=\"{fireWallRuleName }\"");
                    sb.AppendLine($" dir=in action=allow protocol=tcp localport={_localPort}");
                    sb.AppendLine($"echo portfowarding finished");
                    sb.AppendLine($"echo if you try any input, the port will be release");
                    //sb.AppendLine($"pause");

                    //portforwarding 
                    sb.AppendLine("echo portfowarding start");
                    sb.Append($" netsh interface portproxy add v4tov4 ");
                    sb.Append($" listenport={_publicPort} listenaddress={_publicIp} ");
                    sb.AppendLine($"connectport={_localPort} connectaddress={_localIp}");
                    sb.AppendLine($"echo portfowarding finished");
                    sb.AppendLine($"echo if you try any input, the port will be release");
                    sb.AppendLine($"pause");

                    //port release
                    sb.Append($" netsh interface portproxy delete v4tov4 ");
                    sb.AppendLine($"listenport={_localPort} listenaddress= {_localIp}");
                    sb.AppendLine($"echo portforwarding is released. {_localIp}:{_localPort} is free");

                    //close firewall port
                    sb.AppendLine($"netsh advfirewall firewall delete rule name=\"{fireWallRuleName}\"");
                    sb.AppendLine($"echo port closed. {_localIp}:{_localPort}");
                    sb.AppendLine($"echo press any key....");
                    sb.AppendLine($"pause");

                    await ws.WriteLineAsync(sb.ToString());
                }
            }
        }
    }
}
