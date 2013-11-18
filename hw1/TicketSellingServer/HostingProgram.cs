using Registeration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace TicketSellingServer
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {

            if (args.Length != 4)
            {
                Console.WriteLine("Bad arguments");
                Console.WriteLine("TicketSellingServer.exe <port to listen> <search server reg uri> <text data> <seller name>");
                return;
            } 
            string url = null;
            // Check the input:
            try
            {
                url = @"http://" + args[1];
                Convert.ToInt32(args[0]);
                new Uri(url);
                new StreamReader(args[2]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Bad arguments: " + e.Message);
                return;
            }

            // Read arguments
            string address = @"http://localhost:" + args[0] + @"/Services";
            TicketSellingQueryLogic.Instance.Initialize(args[2], args[3]);

            // Create REST client
            ITicketSellerRegistration channel;
            try
            {
                WebChannelFactory<ITicketSellerRegistration> cf = new WebChannelFactory<ITicketSellerRegistration>(new Uri(url));
                channel = cf.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine("Couldn't advertise my URI on the flights search server because:");
                Console.WriteLine(e.Message.ToString());
                return;
            }

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
                catch (Exception e)
                {

                    if (e.InnerException is WebException)
                    {
                        HttpWebResponse resp = (HttpWebResponse)((WebException)e.InnerException).Response;
                        Console.WriteLine("Failed, {0}", resp.StatusDescription);
                    }
                    else
                    {
                        Console.WriteLine("Advertisement connection kicked the bucket, quitting because:");
                        Console.WriteLine(e.Message.ToString());                        
                    }
                    return;
                }

                // Open the service
                host.Open();

                // Keeping the service alive till pressing ENTER
                Console.ReadKey();
            }
        }
    }
}