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
        [OperationContract]
        Flights GetFlights(FlightQuery flightQuery);

        [OperationContract]
        int MakeReservation(FlightSearchReservationRequest request);

        [OperationContract]
        void CancelReservation(string reservationID);

    }
}