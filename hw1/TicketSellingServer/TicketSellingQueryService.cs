using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using System.IO;

namespace TicketSellingServer
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class TicketSellingQueryService : ITicketSellingQueryService
    {
        static int indexer = 0;
        private Flights flights;
        private List<TicketSearchReservation> reservations;
        public TicketSellingQueryService(string filePath)
        {
            reservations = new List<TicketSearchReservation>();
            getFlights(filePath);
        }
        public void getFlights(string filePath){
            flights = new Flights();
            StreamReader reader = new StreamReader(filePath);
            string line = reader.ReadLine();
            while (line != null)
            {
                Flight flight = new Flight();
                string[] members = line.Split(' ');
                flight.flightNumber = members[0];
                flight.src = members[1];
                flight.dst = members[2];
                flight.date = DateTime.Parse(members[3]);
                flight.seats = Convert.ToInt32(members[4]);
                flight.price = Convert.ToInt32(members[5]);
                flights.Add(flight);
                line = reader.ReadLine();
            }
 
        }


        public Flights GetFlights(string src, string dst, string date)
        {
            Flights suitableFlights = new Flights();
            foreach (Flight flight in flights)
            {
                if (flight.src.Equals(src) && flight.dst.Equals(dst))
                {
                    DateTime dt = DateTime.Parse(date);
                    if (dt.Equals(flight.date))
                    {
                        suitableFlights.Add(flight);
                    }
                }
            }

            suitableFlights.Sort();
            return suitableFlights;
        }

        public int MakeReservation(FlightSearchReservationRequest request)
        {
            foreach(Flight flight in flights){
                if (flight.flightNumber.Equals(request.flightNumber) && flight.date.Equals(request.date))
                {
                    if (flight.seats == 0) throw new FaultException("no seats available");
                    flight.seats--;
                    TicketSearchReservation reservation = new TicketSearchReservation();
                    reservation.flightNumber = request.flightNumber;
                    reservation.date = request.date;
                    reservation.reservationID = ++indexer;
                    reservations.Add(reservation);
                    return indexer;
                }
                throw new FaultException("no such flight");
            }
            
            return 0;
        }
        public void CancelReservation(string reservationID)
        {
            foreach (TicketSearchReservation reservation in reservations)
            {
                if (reservation.reservationID == Convert.ToInt32(reservationID))
                {
                    foreach (Flight flight in flights)
                    {
                        if (flight.flightNumber.Equals(reservation.flightNumber) && flight.date.Equals(reservation.date))
                        {
                            flight.seats++;
                        }
                    }
                    reservations.Remove(reservation);
                    return;
                }
            }
            throw new FaultException("no such reservation");
        }

    }
}