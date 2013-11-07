using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
//using FlightSearchServerCA;

namespace TicketSellingServer
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {
            WebChannelFactory<ITicketSellerRegistration> cf = new WebChannelFactory<ITicketSellerRegistration>( new Uri(args[1]));

            ITicketSellerRegistration channel = cf.CreateChannel();
            string address = @"http://localhost:" + args[0] + @"/Services";

            TicketSellingQueryService ticketSelling = new TicketSellingQueryService(args[2]);
            using (ServiceHost host = new ServiceHost(
                ticketSelling, new Uri(address)))
            {
                host.AddServiceEndpoint(typeof(ITicketSellingQueryService), new BasicHttpBinding(), "TicketSellingQueryService"); 

                
                channel.RegisterSeller(new Uri(address), "air-liberman"); 
                host.Open();
                Console.ReadKey();
            }
        }
    }
}