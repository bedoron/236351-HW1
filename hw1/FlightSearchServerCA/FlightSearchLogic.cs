using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TicketSellingServer;


namespace FlightSearchServerCA
{
    /// <summary>
    /// Business logic class of Search server.
    /// this class encapsulates all business logic which. 
    /// </summary>
    public sealed class FlightSearchLogic
    {
        // This is ok as long as we call instance BEFORE anything else
        private static readonly FlightSearchLogic instance = new FlightSearchLogic();

        private FlightSearchLogic() { }

        /// <summary>
        /// Singleton getter/setter
        /// </summary>
        public static FlightSearchLogic Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Associate seller names with their resources
        /// </summary>
        public ConcurrentDictionary<string, ITicketSellingQueryService> sellers =
           new ConcurrentDictionary<string, ITicketSellingQueryService>(Environment.ProcessorCount, Environment.ProcessorCount * 2);

        /// <summary>
        /// Ticket seller registration service.
        /// Publishing mechanism to allow sellers dynamically register our ticekts selling server
        /// </summary>
        private ServiceHost tsrHost;
        /// <summary>
        /// Client search request service
        /// Publishing mechanism to allow clients dynamically make search queries to all registered 
        /// sellers
        /// </summary>
        private ServiceHost cqsHost;
        
        /// <summary>
        /// Boolean variable to indicate if the singleton was initialized
        /// Initialization is sucessful if publishing services started correctly (Didn't throw an exception)
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// Main execution loop variable. program runs as long as this variable is true
        /// </summary>
        private bool continueRunning = true;

        /// <summary>
        /// This function will initialize the publishing services. search server will not run if 
        /// those services didn't initialize correctly.
        /// </summary>
        /// <param name="clientPort">Port for client publishing server</param>
        /// <param name="sellerPort">Port for sellers registration publishing server</param>
        public void Initialize(string clientPort, string sellerPort)
        {
            tsrHost = new ServiceHost(typeof(TicketSellerRegistration), new Uri(@"http://localhost:" + sellerPort + @"/Services/FlightsSearchReg"));
            cqsHost = new ServiceHost(typeof(ClientQueryService), new Uri(@"http://localhost:" + clientPort + @"/Services/FlightsSearch"));

            isInitialized = true;
        }

        /// <summary>
        /// Run servers and serve requests
        /// </summary>
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

        /// <summary>
        /// This function is delegated by the Client Query service. 
        /// it will iterate all sellers and make the appropriate client requested 
        /// query, returns search results to the client
        /// </summary>
        /// <param name="src">Source of flight</param>
        /// <param name="dst">Destination of flight</param>
        /// <param name="date">Date of flight</param>
        /// <returns>Flights from all sellers which match the input criterias</returns>
        public QueryResultFlights QueryFlights(string src, string dst, string date) 
        {
            Console.WriteLine("FlightSearchServer: " + dst + " " + src + " " + date);

            QueryResultFlights flights = new QueryResultFlights();
            foreach (var seller in sellers.Keys)
            {
                FlightQuery fq = new FlightQuery();
                fq.src = src;
                fq.dst = dst;
                fq.date = DateTime.Parse(date);
                using (new OperationContextScope((IContextChannel)sellers[seller]))
                {
                    try
                    {
                        Flights sellerFlights =
                            sellers[seller].GetFlights(fq); // DEAL WITH EXCEPTIONS HERE

                        foreach (var sellerFlight in sellerFlights)
                        {
                            QueryResultFlight f1 = new QueryResultFlight();
                            f1.dst = sellerFlight.dst;
                            f1.src = sellerFlight.src;
                            f1.seats = sellerFlight.seats;
                            f1.price = sellerFlight.price;
                            f1.name = seller;
                            f1.flightNumber = sellerFlight.flightNumber;
                            f1.date = sellerFlight.date;
                            flights.Add(f1);

                        }

                    }
                    catch (FaultException e)
                    {
                        FlightSearchServerException fsse = new FlightSearchServerException(e.Reason.ToString());
                        Console.WriteLine("Seller {0} failed with {1}", seller, e.Reason.ToString());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Seller {0} {1} malfunction: \n{2}", seller, "search", e.Message.ToString());
                        ITicketSellingQueryService victim;
                        sellers.TryRemove(seller, out victim);
                    }
                }
            }

            flights.Sort();

            return flights;
        }

        /// <summary>
        /// Delegate function to make reservation. 
        /// this function will make a reservation with the requested seller
        /// if seller doesn't exist or ReservationRequest is invalid on seller's
        /// server an excetion will be throwed.
        /// </summary>
        /// <param name="seller">Seller to order from</param>
        /// <param name="request">Request parameters to the sellers</param>
        /// <returns>sucessfull reservation ID</returns>
        public int MakeReservation(string seller, ReservationRequest request)
        {
            if(!sellers.ContainsKey(seller)) 
            {
                throw new FlightSearchServerException("unknown seller");
            }
            FlightSearchReservationRequest fsrr = 
                new FlightSearchReservationRequest();

            // Prepare data
            fsrr.date = request.date;
            fsrr.flightNumber = request.flightNumber;

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

        /// <summary>
        /// Delegate function to cancel reservation
        /// this function will delegate the reservation ID to the appropriate seller.
        /// if seller doesn't exist or reservation ID is invalid an exception will be thrown
        /// </summary>
        /// <param name="seller">Seller to delegate this request</param>
        /// <param name="reservationID">re</param>
        public void CancelReservation(string seller, string reservationID)
        {
            int resID;
            try
            {
                resID = Convert.ToInt32(reservationID);
            }
            catch (FormatException e)
            {
                throw new FlightSearchServerException("bad reservation ID: " + e.Message); 
            }
            if (!sellers.ContainsKey(seller))
            {
                throw new FlightSearchServerException("unknown seller");
            }
            FlightSearchReservationRequest fsrr =
                new FlightSearchReservationRequest();
            try
            {
                sellers[seller].CancelReservation(resID);
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
