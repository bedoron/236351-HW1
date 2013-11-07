using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace TicketSellingServer
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(
                typeof(TicketSellingQueryService), new Uri("http://localhost:50001/Services")))
            {
                
                host.Open();
                Console.ReadKey();
            }
        }
    }
}