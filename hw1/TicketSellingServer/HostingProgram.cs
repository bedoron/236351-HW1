using Registeration;
using System;
using System.Collections.Generic;
using System.IO;
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
            // Check the input:
            try
            {
                Convert.ToInt32(args[0]);
                new Uri(args[1]);
                new StreamReader(args[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Bad arguments: " + e.Message);
            }

            // Read arguments
            string address = @"http://localhost:" + args[0] + @"/Services";
            TicketSellingQueryLogic.Instance.Initialize(args[2], args[3]);

            // Create REST client
            WebChannelFactory<ITicketSellerRegistration> cf = new WebChannelFactory<ITicketSellerRegistration>(new Uri(args[1]));
            ITicketSellerRegistration channel = cf.CreateChannel();

            using (ServiceHost host = new ServiceHost(
                typeof(TicketSellingQueryService), new Uri(address)))
            {
                // Create SOAP client
                host.AddServiceEndpoint(typeof(ITicketSellingQueryService), new BasicHttpBinding(), "TicketSellingQueryService");

                try
                {
                    // Register the channel in the server
                    channel.RegisterSeller(new Uri(address), args[3]);
                }
                catch (ProtocolException e)
                {
                    Console.WriteLine("Bad Protocol: " + e.Message);
                }

                // Open the service
                host.Open();

                // Keeping the service alive till pressing ENTER
                Console.ReadKey();
            }
        }
    }
}