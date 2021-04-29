using common.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server.Utils
{
    public class ServerMonitor
    {
        //모니터링 시 필요한 정보 : process 이름 
        // cpu 사용량 : 머신 / 현재 프로세스 
        // mem 사용량 : 머신 / 현재 프로세스
        private PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter ram = new PerformanceCounter("Memory", "Available MBytes");

        private static string processorName = Process.GetCurrentProcess().ProcessName;

        private PerformanceCounter process_cpu = new PerformanceCounter("Processor", "% Processor Time", processorName);

        private bool loop_state = true;

        private CoreLogger logger = new ConsoleLogger();

        public void UpdateMonitor()
        {
        }

        [Conditional("DEBUG")]
        public void Render()
        {
            var sb = new StringBuilder();
            sb.AppendLine("[ServerMonitor]");
            sb.AppendLine($"cur Total Cpu : {}");
            sb.AppendLine($"Cur Use Cpu :  {}");
            sb.AppendLine($"cur process use cpu :  {}");

            sb.AppendLine($"Cur Use Mem :  {}");
            sb.AppendLine($"cur process use Mem :  {}");

            logger.WriteDebug(sb.ToString());
        }
    }
}
