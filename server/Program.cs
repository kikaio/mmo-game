using common.Utils.Loggers;
using server.Utils;
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
            Task.Factory.StartNew(async () => {

                var netInfo = await IPUtils.GetPublicIpStr();
                netInfo.public_port = 30000;
                netInfo.private_port = 30000;

                //todo : should be move this setting to App.config
                logger.WriteDebug("batch file creating started");
                string folderPath = @".";
                string batchName = "portfowarding.bat";
                try
                {
                    await FileUtils.PortfowardBatchCreate(folderPath, batchName
                        , netInfo.public_Ip, netInfo.private_Ip
                        , netInfo.public_port, netInfo.private_port);
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                }
                logger.WriteDebug("batch file create complete");
            });

            while (mServer.IsShutdownRequested() == false)
            {
                Thread.Sleep(1000);
            }
            Console.WriteLine("programe down, press any key");

        }
    }
}
