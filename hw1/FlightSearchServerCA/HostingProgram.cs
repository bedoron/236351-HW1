using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace FlightSearchServerCA
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {
            TicketSellerRegistration regiteration = new TicketSellerRegistration();
            using (ServiceHost host = new ServiceHost(
                regiteration, new Uri(@"http://localhost:"+args[1]+@"/Services")))
            {
                host.Open();
                
                Console.ReadKey();
            }
        }
    }
}