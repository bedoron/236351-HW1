using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TicketSellingServer
{
    [DataContract]
    public class FlightSearchReservationRequest
    {
        [DataMember]
        public string flightNumber{ get; set;}
        [DataMember]
        public DateTime date{ get; set;}

    }

    [DataContract]
    public class FlightQuery
    {
        [DataMember]
        public string src { get; set; }
        [DataMember]
        public string dst { get; set; }
        [DataMember]
        public string date { get; set; }

    }


    
    public class TicketSearchReservation
    {
        public int reservationID;
        public DateTime date;
        public string flightNumber;
    }

    [CollectionDataContract]
    public class Flights : List<Flight>
    {

        public Flights() { }
        public Flights(List<Flight> flights) : base(flights) { }
    }
    [DataContract]
    public class Flight : IComparable
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


        public int CompareTo(object obj)
        {
            Flight otherFlight = (Flight)obj;
            if (price < otherFlight.price) { return -1; }
            else if (price > otherFlight.price) { return 1; }
            else
            {
                if (seats > otherFlight.seats) { return -1; }
                else if (seats < otherFlight.seats) { return 1; }
                else { return flightNumber.CompareTo(otherFlight.flightNumber); }
            }
        }
    }


}