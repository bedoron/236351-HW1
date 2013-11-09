using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace FlightSearchServerCA
{
    [DataContract]
    public class ReservationRequest
    {
        [DataMember]
        public string flightNumber{ get; set;}
        [DataMember]
        public DateTime date{ get; set;}

    }

    [CollectionDataContract]
    public class Flights1 : List<Flight1>
    {

        public Flights1() { }
        public Flights1(List<Flight1> flights) : base(flights) { }
    }
    [DataContract]
    public class Flight1
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
    }

    
}