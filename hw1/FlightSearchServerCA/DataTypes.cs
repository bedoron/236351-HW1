using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlightSearchServerCA
{
    /// <summary>
    /// Describe a reservation from client
    /// </summary>
    [DataContract]
    public class ReservationRequest
    {
        [DataMember]
        public string flightNumber{ get; set;}
        [DataMember]
        public DateTime date{ get; set;}

    }

    /// <summary>
    /// Collection of flights which were retrieved by search queries who where delegated
    /// to all the sellers.
    /// This list is propogated to the client as a result
    /// </summary>
    [CollectionDataContract]
    public class QueryResultFlights : List<QueryResultFlight>
    {

        public QueryResultFlights() { }
        public QueryResultFlights(List<QueryResultFlight> flights) : base(flights) { }
    }

    /// <summary>
    /// A single Flight search query result. this class will be contained within the list
    /// QueryResultFlights.
    /// </summary>
    [DataContract]
    public class QueryResultFlight : IComparable
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string flightNumber { get; set; }
        [DataMember]
        public string src { get; set; }
        [DataMember]
        public string dst { get; set; }
        [DataMember]
        public int seats { get; set; }
        [DataMember]
        public int price { get; set; }
        [DataMember]
        public DateTime date { get; set; }

        /// <summary>
        /// This function is used to sort the list by spec requirement.
        /// Client is dumb so sorting is done on server
        /// </summary>
        /// <param name="obj">Right hand object to test</param>
        /// <returns>-1 if this is smaller than obj, otherwise 1</returns>
        public int CompareTo(object obj)
        {
            QueryResultFlight otherFlight = (QueryResultFlight)obj;

            if (price < otherFlight.price) { return -1; }
            else if (price > otherFlight.price) { return 1; }
            else
            {
                if (seats > otherFlight.seats) { return -1; }
                else if (seats < otherFlight.seats) { return 1; }
                else { return flightNumber.CompareTo(otherFlight.flightNumber); }
            }
        }


        public static explicit operator QueryResultFlight(TicketSellingServer.Flight sellerFlight)
        {
            QueryResultFlight clientFlight = new QueryResultFlight();
            clientFlight.dst = sellerFlight.dst;
            clientFlight.src = sellerFlight.src;
            clientFlight.seats = sellerFlight.seats;
            clientFlight.price = sellerFlight.price;
            clientFlight.name = "UNKNOWN_SELLER_WITH_A_REALLY_REALLY_LONG_NAME_WHICH_NOBODY_CARES_ABOUT";
            clientFlight.flightNumber = sellerFlight.flightNumber;
            clientFlight.date = sellerFlight.date;

            return clientFlight;
        }
    }

    
}