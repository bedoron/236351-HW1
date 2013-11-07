using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using TicketSellingServer;
using System.ServiceModel.Web;

namespace FlightSearchServerCA
{
    class TicketSellerRegistration : ITicketSellerRegistration
    {
        public ConcurrentDictionary<string, ITicketSellingQueryService> sellers { get; set; }
        public TicketSellerRegistration()
        {
            sellers = new ConcurrentDictionary<string, ITicketSellingQueryService>(Environment.ProcessorCount, Environment.ProcessorCount * 2);
        }
        public void RegisterSeller(Uri request, string name)
        {
            WebChannelFactory<ITicketSellingQueryService> cf = new WebChannelFactory<ITicketSellingQueryService>(request);

            ITicketSellingQueryService channel = cf.CreateChannel();
            sellers[name] = channel;
        }
    }
}
