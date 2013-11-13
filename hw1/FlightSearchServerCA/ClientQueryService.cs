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
    /// This class implements the ClientQueryService handlers as defined by the
    /// Contract API
    /// </summary>
    public class ClientQueryService : IClientQueryService
    {
        /// <summary>
        /// client flights search query handler
        /// This function will call the business logic singleton to fetch Flights
        /// from all sellers, convert them to the appropriate container and return 
        /// them to the client.
        /// </summary>
        /// <param name="src">Source of the flight</param>
        /// <param name="dst">Destination of the flight</param>
        /// <param name="date">Reuired date</param>
        /// <returns>List of flights ordered as requested</returns>
        public QueryResultFlights GetFlights(string src, string dst, string date)
        {
            Console.WriteLine("ClientQueryService: "+dst+" "+src+" "+date);
            Flights flights = null;
            try
            {
                flights = FlightSearchServer.Instance.QueryFlights(src, dst, date);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }

            QueryResultFlights fs = new QueryResultFlights();
            foreach (Flight f in flights)
            {
                QueryResultFlight f1 = new QueryResultFlight();
                f1.dst = f.dst;
                f1.src = f.src;
                f1.seats = f.seats;
                f1.name = f.name;
                f1.flightNumber = f.flightNumber;
                f1.date = f.date;
                fs.Add(f1);
            }

            fs.Sort();
            return fs;
        }

        /// <summary>
        /// Client reservation request handler.
        /// Delegate the requeset to the appropriate seller and return the result
        /// if request or seller doesn't exist throw an exception
        /// </summary>
        /// <param name="seller">Seller to delegate this request</param>
        /// <param name="request">reservetaion request parameters from user</param>
        /// <returns>reservation id from seller</returns>
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

        /// <summary>
        /// Client reservation cancelation handler
        /// Delegate the reservation id to the appropriate seller.
        /// if reservation or seller doesn't exist throw an exception to the 
        /// client
        /// </summary>
        /// <param name="seller">Seller to delegate this request</param>
        /// <param name="CancelReservation">reservation id as a string to cancel</param>
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