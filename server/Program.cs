using common.Utils.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreLogger logger = new Log4Logger();
            int port = 30000;
            foreach (var ele in args)
            {
                string[] splitStr = ele.Split(',');
                foreach (var str in splitStr)
                {
                    //ex : port:12345
                    if (str.ToLower().StartsWith("port"))
                    {
                        string[] tmp = str.Split(':');
                        if (tmp.Length == 2)
                            int.TryParse(tmp[1], out port);
                    }
                }
            }

            MmoServer mServer = new MmoServer(port);
            mServer.ReadyToStart();
            mServer.Start();
            while (mServer.IsShutdownRequested() == false)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("programe down, press any key");
        }
    }
}
