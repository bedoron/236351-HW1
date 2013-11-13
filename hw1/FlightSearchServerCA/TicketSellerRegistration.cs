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
using System.ServiceModel.Description;


namespace FlightSearchServerCA
{
    public class TicketSellerRegistration : ITicketSellerRegistration
    {

        public void RegisterSeller(Uri request, string name)
        {
            ChannelFactory<ITicketSellingQueryService> httpFactory = new ChannelFactory<ITicketSellingQueryService>(new ServiceEndpoint(ContractDescription.GetContract(typeof(ITicketSellingQueryService)), new BasicHttpBinding(), new EndpointAddress(request)));
            // create channel proxy for endpoint
            ITicketSellingQueryService channel = httpFactory.CreateChannel();
            FlightSearchLogic.Instance.sellers[name] = channel;

        }
    }
}
