using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using TicketSellingServer;
using System.ServiceModel.Web;
using System.ServiceModel;

namespace FlightSearchServerCA
{
    public class TicketSellerRegistration : ITicketSellerRegistration
    {
        public void RegisterSeller(Uri request, string name)
        {
            WebChannelFactory<ITicketSellingQueryService> cf = new WebChannelFactory<ITicketSellingQueryService>(request);

            ITicketSellingQueryService channel = cf.CreateChannel();
            FlightSearchServer.Instance.sellers[name] = channel;
        }
    }
}
