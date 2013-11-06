using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace FlightSearchServer
{

    public class ClientQueryService : IClientQueryService
    {
        public Flights GetFlights(string src, string dst, string date)
        {
            //TODO: implement...
            return null;
        }

        public int MakeReservation(string seller, ReservationRequest request)
        {
            //TODO: implement - return the ID of the new resevation
            return 0;
        }
        public void CancelReservation(string seller, int reservationID)
        {
     
     
        }

    }
}