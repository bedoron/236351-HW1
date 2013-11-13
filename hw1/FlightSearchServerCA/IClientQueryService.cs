using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using TicketSellingServer;

namespace FlightSearchServerCA
{
    /// <summary>
    /// Defines the API between Client and Search server
    /// </summary>
    [ServiceContract]
    public interface IClientQueryService
    {
        /// <summary>
        /// See concrete class for details
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "flight?src={src}&dst={dst}&date={date}")]
        [OperationContract]
        QueryResultFlights GetFlights(string src, string dst, string date);

        /// <summary>
        /// See concrete class for details
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(Method = "POST", UriTemplate = "seller/{seller}")]
        [OperationContract]
        int MakeReservation(string seller, ReservationRequest request);

        /// <summary>
        /// See concrete class for details
        /// </summary>
        /// <param name="seller"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [WebInvoke(Method = "DELETE", UriTemplate = "seller/{seller}/{reservationID}")]
        [OperationContract]
        void CancelReservation(string seller, string reservationID);

    }
}