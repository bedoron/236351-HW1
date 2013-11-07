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
            Flights queryResult = new Flights();
            //FlightSearchServer.Instance.
            return queryResult;
            //Console.WriteLine("PIHO.....................................................");
            //Flight flight = new Flight();
            //flight.date = new DateTime(1212, 12, 12);
            //flight.dst = "SUDAN";
            //flight.flightNumber = "AL45";
            //flight.name = "Air-Liberman";
            //flight.price = 100;
            //flight.seats = 4096;
            //flight.src = "TLV";

            //Flights flights = new Flights();
            //flights.Add(flight);
            //return flights;
        }

        public int MakeReservation(string seller, ReservationRequest request)
        {
            Console.WriteLine("Seller: {0}\n\tdate: {1}\n\tflight: {2}",seller, request.date.ToString(), request.flightNumber);
            return 2046;
        }
        public void CancelReservation(string seller, string reservationID)
        {
            //WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound("Invalid seller");
        }

    }
}