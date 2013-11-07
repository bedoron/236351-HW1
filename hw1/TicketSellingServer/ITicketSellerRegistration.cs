using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchServerCA
{
    [ServiceContract]
    public interface ITicketSellerRegistration
    {
        [WebInvoke(Method = "PUT", UriTemplate = "registeration")]
        [OperationContract]
        void RegisterSeller(Uri request, string name);
    }
}