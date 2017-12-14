using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TrueMarbleBiz
{
    class Program
    {
        static void Main(string[] args)
        {
            //sets up the server connection
            NetTcpBinding tcpBinding = new NetTcpBinding();
            //sets max value
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
            ServiceHost host = new ServiceHost(typeof(TMBizControllerImpl));

            try
            {
                host.AddServiceEndpoint(typeof(ITMBizController), tcpBinding, "net.tcp://localhost:50002/TMBiz");
            }
            catch (FaultException)
            {
                Console.WriteLine("Fault Exception thrown in Program.cs > Main");
            }

            //opens connection
            host.Open();
            System.Console.WriteLine("Press Enter to Exit");
            //So server doesn't exit before it can service clients
            System.Console.ReadLine();
            //closes connection
            host.Close();
        }
    }
}
