using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace TicketSellingServer
{
    [ServiceContract]
    public interface ITicketSellingQueryService
    {
        [WebGet(UriTemplate = "flight?src={src}&dst={dst}&date={date}")]
        [OperationContract]
        Flights GetFlights(string src, string dst, string date);

        [WebInvoke(Method = "POST", UriTemplate = "makeReservation")]
        [OperationContract]
        int MakeReservation(FlightSearchReservationRequest request);

        [WebInvoke(Method = "DELETE", UriTemplate = "cancelReservation")]
        [OperationContract]
        void CancelReservation(string reservationID);

    }
}