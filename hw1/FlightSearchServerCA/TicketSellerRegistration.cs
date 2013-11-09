using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.ServiceModel.Web;
using System.ServiceModel;
using TicketSellingServer;
using Registeration;


namespace FlightSearchServerCA
{
    public class TicketSellerRegistration : ITicketSellerRegistration
    {
        public void RegisterSeller(Uri request, string name)
        {
            ChannelFactory<ITicketSellingQueryService> httpFactory =
        new ChannelFactory<ITicketSellingQueryService>("BasicHttpBinding_ITicketSellingQueryService");
                // create channel proxy for endpoint
                ITicketSellingQueryService channel = httpFactory.CreateChannel();
                FlightSearchServer.Instance.sellers[name] = channel;

                Console.WriteLine("PIHO11!!!!!!!!!!!!!!!!");
            

        }
    }
}
