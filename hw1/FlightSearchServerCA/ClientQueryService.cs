using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;

namespace FlightSearchServerCA
{

    public class ClientQueryService : IClientQueryService
    {

        public Flights GetFlights(string src, string dst, string date)
        {
            Flights flights = null;
            try
            {
                flights = FlightSearchServer.Instance.QueryFlights(src, dst, date);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }

            return flights;
        }

        public int MakeReservation(string seller, ReservationRequest request)
        {
            int reservationID = 0;
            try
            {
                reservationID = FlightSearchServer.Instance.MakeReservation(seller, request);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }

            return reservationID;
        }
        public void CancelReservation(string seller, string CancelReservation)
        {
            try
            {
                FlightSearchServer.Instance.CancelReservation(seller, CancelReservation);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }

        }

    }
}