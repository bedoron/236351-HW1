using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchServerCA
{
    public sealed class FlightSearchServer
    {
        // This is ok as long as we call instance BEFORE anything else
        private static readonly FlightSearchServer instance = new FlightSearchServer();

        private FlightSearchServer() { }

        public static FlightSearchServer Instance
        {
            get
            {
                return instance;
            }
        }

        public ConcurrentDictionary<string, ITicketSellingQueryService> sellers =
            new ConcurrentDictionary<string, ITicketSellingQueryService>(Environment.ProcessorCount, Environment.ProcessorCount * 2);

        private ServiceHost tsrHost;
        private ServiceHost cqsHost;
        
        private bool isInitialized = false;
        private bool continueRunning = true;

        public void Initialize(string clientPort, string sellerPort)
        {
            tsrHost = new ServiceHost(typeof(TicketSellerRegistration), new Uri(@"http://localhost:" + sellerPort + @"/Services"));
            cqsHost = new ServiceHost(typeof(ClientQueryService), new Uri(@"http://localhost:" + clientPort + @"/Services"));

            isInitialized = true;
        }

        public void run()
        {
            if (!isInitialized)
                return;

            Console.WriteLine("Running");
            try
            {
                tsrHost.Open();
                cqsHost.Open();
            }
            catch (Exception)
            {
                if (tsrHost != null)
                    tsrHost.Close();
                if (cqsHost != null)
                    cqsHost.Close();
            }
            while (continueRunning)
            {
                System.Threading.Thread.Sleep(5000);
                // Maybe do some cleanup on sellers ? 
            }
        }
    }
}
