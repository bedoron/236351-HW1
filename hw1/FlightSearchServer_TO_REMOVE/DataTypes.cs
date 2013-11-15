using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlightSearchServer
{
    [DataContract]
    public class ReservationRequest
    {
        [DataMember]
        public Uri id { get; set; }
        [DataMember]
        public string flightNumber{ get; set;}
        [DataMember]
        public DateTime date{ get; set;}

    }
    [DataContract]
    public class Flight
    {
        [DataMember]
        public Uri id { get; set; }
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

    }

    [CollectionDataContract]
    public class Flights : List<Flight>
    {
        public Flights() { }
        public Flights(List<Flight> flights) : base(flights) { }
    }

}