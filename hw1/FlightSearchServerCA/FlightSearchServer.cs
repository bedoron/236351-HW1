using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchServerCA
{
    public sealed class FlightSearchServer
    {
        // This is ok as long as we call instance BEFORE anything else
        private static readonly FlightSearchServer instance = new FlightSearchServer();

        private FlightSearchServer() { }

        public static FlightSearchServer Instance
        {
            get
            {
                return instance;
            }
        }

        public ConcurrentDictionary<string, ITicketSellingQueryService> sellers =
            new ConcurrentDictionary<string, ITicketSellingQueryService>(Environment.ProcessorCount, Environment.ProcessorCount * 2);

        private ServiceHost tsrHost;
        private ServiceHost cqsHost;
        
        private bool isInitialized = false;
        private bool continueRunning = true;

        public void Initialize(string clientPort, string sellerPort)
        {
            tsrHost = new ServiceHost(typeof(TicketSellerRegistration), new Uri(@"http://localhost:" + sellerPort + @"/Services"));
            cqsHost = new ServiceHost(typeof(ClientQueryService), new Uri(@"http://localhost:" + clientPort + @"/Services"));

            isInitialized = true;
        }

        public void run()
        {
            if (!isInitialized)
                return;

            Console.WriteLine("Running");
            try
            {
                tsrHost.Open();
                cqsHost.Open();
            }
            catch (Exception)
            {
                if (tsrHost != null)
                    tsrHost.Close();
                if (cqsHost != null)
                    cqsHost.Close();
            }
            while (continueRunning)
            {
                System.Threading.Thread.Sleep(5000);
                // Maybe do some cleanup on sellers ?
            }
        }

        public Flights QueryFlights(string src, string dst, string date) 
        {
            Flights flights = new Flights();
            foreach (var seller in sellers)
            {
                try
                {
                    TicketSellingServer.Flights sellerFlights =
                        seller.Value.GetFlights(src, dst, date); // DEAL WITH EXCEPTIONS HERE
                    foreach (var sellerFlight in sellerFlights)
                    {
                        Flight flight = new Flight();
                        flight.date = sellerFlight.date;
                        flight.dst = sellerFlight.dst;
                        flight.flightNumber = sellerFlight.flightNumber;
                        flight.price = sellerFlight.price;
                        flight.seats = sellerFlight.seats;
                        flight.src = sellerFlight.src;

                        flight.name = seller.Key;
                        flights.Add(flight);
                    }
                }
                catch (FaultException e)
                {
                    FlightSearchServerException fsse = new FlightSearchServerException(e.Reason.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Seller {0} {1} malfunction: \n{2}", seller.Key, "search", e.Message.ToString());
                    ITicketSellingQueryService victim;
                    sellers.TryRemove(seller.Key, out victim);
                }
            }
            return flights;
        }

        public int MakeReservation(string seller, ReservationRequest request)
        {
            if(!sellers.ContainsKey(seller)) 
            {
                throw new FlightSearchServerException("unknown seller");
            }
            TicketSellingServer.FlightSearchReservationRequest fsrr = 
                new TicketSellingServer.FlightSearchReservationRequest();
            int reservationID = 0;    
            try {
                reservationID = sellers[seller].MakeReservation(fsrr);
            } 
            catch(FaultException e) 
            {
                throw new FlightSearchServerException(e.Reason.ToString());
            } 
            catch(Exception e) 
            {
                Console.WriteLine("Seller {0} {1} malfunction: \n{2}", seller, "Make reservation", e.Message.ToString());
                ITicketSellingQueryService victim;
                sellers.TryRemove(seller, out victim);
            }
            return reservationID;
        }

        public void CancelReservation(string seller, string reservationID)
        {
            if (!sellers.ContainsKey(seller))
            {
                throw new FlightSearchServerException("unknown seller");
            }
            TicketSellingServer.FlightSearchReservationRequest fsrr =
                new TicketSellingServer.FlightSearchReservationRequest();
            try
            {
                sellers[seller].CancelReservation(reservationID);
            }
            catch (FaultException e)
            {
                throw new FlightSearchServerException(e.Reason.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Seller {0} {1} malfunction: \n{2}", seller, "Cancel reservation",e.Message.ToString());
                ITicketSellingQueryService victim;
                sellers.TryRemove(seller, out victim);
            }
        }

    }
}
