//Fatema Shabbir 
//19201960

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.ServiceModel;

namespace TrueMarbleData
{
    class Program
    {
        public static void Main(string[] args)
        {
            TMDataControllerImpl dc = new TMDataControllerImpl();

            //sets up the server connection
            NetTcpBinding tcpBinding = new NetTcpBinding();
            //sets max value
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
            ServiceHost host = new ServiceHost(dc);
            
            try 
            {    
                host.AddServiceEndpoint(typeof(ITMDataController), tcpBinding, "net.tcp://localhost:50001/TMData");
            }
            catch (FaultException) 
            {
                Console.WriteLine("Fault Exception thrown in Program.cs > Main");
            }

            //opens connection
            host.Open();
            System.Console.WriteLine("Press Enter to Exit");
            System.Console.ReadLine();
            //closes connection
            host.Close();
        }
    }
}
