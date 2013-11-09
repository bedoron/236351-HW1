using Registeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace TicketSellingServer
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {
            WebChannelFactory<ITicketSellerRegistration> cf = new WebChannelFactory<ITicketSellerRegistration>(new Uri(args[1]));

            ITicketSellerRegistration channel = cf.CreateChannel();
            string address = @"http://localhost:" + args[0] + @"/Services";

            TicketSellingQueryService ticketSelling = new TicketSellingQueryService(args[2], args[3]);
            using (ServiceHost host = new ServiceHost(
                ticketSelling, new Uri(address)))
            {
                host.AddServiceEndpoint(typeof(ITicketSellingQueryService), new BasicHttpBinding(), "TicketSellingQueryService");

                try
                {
                    channel.RegisterSeller(new Uri(address), args[3]);
                }
                catch (ProtocolException e) { Console.WriteLine(e.Message); }
                host.Open();
                Console.ReadKey();
            }
        }
    }
}