using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace TicketSellingServer
{
    public sealed class TicketSellingQueryLogic
    {
        /// <summary>
        /// The name of this seller.
        /// </summary>
        private string sellerName;

        /// <summary>
        /// Static member for reservation ID creator.
        /// </summary>
        static int reservationIdCreator = 0;

        /// <summary>
        /// List of flights offered by this seller.
        /// </summary>
        private Flights flights;

        /// <summary>
        /// List of the reservations that were made with this seller.
        /// </summary>
        private List<TicketSearchReservation> reservations;

        /// <summary>
        /// Initialize - must be called first!
        /// </summary>
        public void Initialize(string filePath, string name)
        {
            sellerName = name;     
            ParseTextFile(filePath);

        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        private static TicketSellingQueryLogic instance;

        /// <summary>
        /// CTOR
        /// </summary>
        private TicketSellingQueryLogic()
        {
            //Create reservations list
            reservations = new List<TicketSearchReservation>();
        }

        /// <summary>
        /// Singleton's getInstance
        /// </summary>
        public static TicketSellingQueryLogic Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TicketSellingQueryLogic();
                }
                return instance;
            }
        }

        /// <summary>
        /// Parse the input file - fill the flights database
        /// </summary>
        /// <param name="filePath">The input file (get flights)</param>
        public void ParseTextFile(string filePath)
        {
            flights = new Flights();
            StreamReader reader = new StreamReader(filePath);
            string line = reader.ReadLine();

            // Read each line and create a new flight
            while (line != null)
            {

                // Create the flight
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


        /// <summary>
        /// Gets a query and returns the suitable flights
        /// </summary>
        /// <param name="flightQuery">the query</param>
        /// <returns>returns the needed flights</returns>
        public Flights GetFlights(FlightQuery flightQuery)
        {
            Flights suitableFlights = new Flights();
            foreach (Flight flight in flights)
            {
                // Checks if the flight fits the query
                if (flight.src.Equals(flightQuery.src) && flight.dst.Equals(flightQuery.dst))
                {
                    if (flightQuery.date.Equals(flight.date))
                    {
                        suitableFlights.Add(flight);
                    }
                }
            }

            return suitableFlights;
        }

        /// <summary>
        /// Make an order for the client.
        /// </summary>
        /// <param name="request">The reservation request</param>
        /// <returns>reservation ID</returns>
        public int MakeReservation(FlightSearchReservationRequest request)
        {
            // Search for the flight
            foreach (Flight flight in flights)
            {
                if (flight.flightNumber.Equals(request.flightNumber) && flight.date.Equals(request.date))
                {
                    // if there is no place in the plane - error!
                    if (flight.seats == 0) throw new Exception("no seats available");

                    // Reduce seats
                    flight.seats--;

                    // Build a reservation
                    TicketSearchReservation reservation = new TicketSearchReservation();
                    reservation.flightNumber = request.flightNumber;
                    reservation.date = request.date;
                    reservation.reservationID = ++reservationIdCreator;
                    reservations.Add(reservation);
                    return reservation.reservationID;
                }
                throw new Exception("no such flight");
            }

            return 0;
        }

        /// <summary>
        /// Removes an order from the system
        /// </summary>
        /// <param name="reservationID">The reservation ID</param>
        public void CancelReservation(int reservationID)
        {
            foreach (TicketSearchReservation reservation in reservations)
            {
                // Check if this is the reservation
                if (reservation.reservationID == reservationID)
                {
                    foreach (Flight flight in flights)
                    {
                        // Check if this is the relevant flight
                        if (flight.flightNumber.Equals(reservation.flightNumber) && flight.date.Equals(reservation.date))
                        {
                            // There is an available seat.
                            flight.seats++;
                        }
                    }
                    reservations.Remove(reservation);
                    return;
                }
            }
            // No reservation found
            throw new Exception("no such reservation");
        }

    }
}