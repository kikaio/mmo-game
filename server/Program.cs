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

            MmoServer mServer = new MmoServer();
            mServer.ReadyToStart();
            mServer.Start();
            while (mServer.IsShutdownRequested() == false)
            {
                Thread.Sleep(1000);
            }

            Console.WriteLine("programe down, press any key");
            Console.ReadKey();
        }
    }
}
