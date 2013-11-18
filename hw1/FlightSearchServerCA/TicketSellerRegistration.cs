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
using System.Net;

namespace FlightSearchServerCA
{
    public class TicketSellerRegistration : ITicketSellerRegistration
    {

        public void RegisterSeller(Uri request, string name)
        {
            if(FlightSearchLogic.Instance.sellers.ContainsKey(name)) {
                // Send an error to the seller: 400 - Bad request, user should reformat
                Console.WriteLine("seller {0} requested by {1} is already taken, Forbidden", name, request.ToString());
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = name + " name is already taken, choose a different one";
            } else {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
                // 1. get rid of stale connections which this seller might have done
                foreach (var seller in FlightSearchLogic.Instance.sellers)
                {
                    IClientChannel currChannel = ((IClientChannel)seller.Value);
                    
                    if (currChannel.RemoteAddress.Uri.Equals(request))
                    {
                        Console.WriteLine("Detected connection retry by {0} from {1} ,removing old connection", name, request.ToString());
                        // Close this channel
                        try
                        {
                            currChannel.Close();
                            currChannel.Abort();
                        }
                        catch (Exception)
                        {
                            currChannel.Abort();
                            Console.WriteLine("Closing of stale channel {0} failed, ignoring", currChannel.RemoteAddress.Uri.ToString());
                        }
                        ITicketSellingQueryService victimChannel;
                        FlightSearchLogic.Instance.sellers.TryRemove(seller.Key, out victimChannel);
                    }
                }
                // 2. Add this seller.
                ChannelFactory<ITicketSellingQueryService> httpFactory = new ChannelFactory<ITicketSellingQueryService>(new ServiceEndpoint(ContractDescription.GetContract(typeof(ITicketSellingQueryService)), new BasicHttpBinding(), new EndpointAddress(request)));
                // create channel proxy for endpoint
                ITicketSellingQueryService channel = httpFactory.CreateChannel();
                FlightSearchLogic.Instance.sellers[name] = channel;
                Console.WriteLine("seller {0} from {1} registered successfully", name, request.ToString());
            }
        }
    }
}
