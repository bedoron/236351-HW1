using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace FlightSearchServerCA
{
    [ServiceContract]
    public interface IClientQueryService
    {
        [WebGet(UriTemplate = "flight?src={src}&dst={dst}&date={date}")]
        [OperationContract]
        Flights GetFlights(string src, string dst, string date);

        [WebInvoke(Method = "POST", UriTemplate = "seller/{seller}")]
        [OperationContract]
        int MakeReservation(string seller, ReservationRequest request);

        [WebInvoke(Method = "DELETE", UriTemplate = "seller/{seller}/{reservationID}")]
        [OperationContract]
        void CancelReservation(string seller, string reservationID);

    }
}