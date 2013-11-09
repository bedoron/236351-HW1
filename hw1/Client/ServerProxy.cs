using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Globalization;
using System.ServiceModel;
using System.Net;
using TicketSellingServer;
using FlightSearchServerCA;

namespace Client
{
    class ServerProxy
    {
        WebChannelFactory<FlightSearchServerCA.IClientQueryService>
            cf;

        Uri serverUri;
        FlightSearchServerCA.IClientQueryService channel;

        private static DateTime GetDate(string strDate) {
            DateTime date = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            return date;
        }

        public ServerProxy(string uri)
        {
            serverUri = new Uri(uri);
            Console.WriteLine("Connection to: {0}", uri);
            cf = new WebChannelFactory<FlightSearchServerCA.IClientQueryService>(serverUri);
            channel = cf.CreateChannel();
        }

        public void run()
        {
            try
            {
                executionLoop();
            }
            catch (EndpointNotFoundException)
            {
                Console.WriteLine("Ticket Selling Server is down");
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.Message.ToString());
                Console.WriteLine("=========================================");
                Console.WriteLine("{0}", e.GetType());
            }
        }

        private void executionLoop() {
            Console.WriteLine("Enter a command or q to quit");
            do
            {
                Console.Write(">");
                string line = Console.ReadLine();
                if (line != null)
                {
                    string[] parameters = line.Split(' ');
                    string command = parameters[0].ToLower();

                    try
                    {
                        if (command.Equals(""))
                        {
                            continue;
                        }
                        else if (command.Equals("search"))
                        {
                            search(parameters);
                        }
                        else if (command.Equals("reserve"))
                        {
                            reserve(parameters);
                        }
                        else if (command.Equals("cancel"))
                        {
                            cancel(parameters);
                        }
                        else if (command.Equals("q"))
                        {
                            return;
                        }
                        else
                        {
                            Console.WriteLine("Invalid command");
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("Failed creating WCF Object");
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid time format, use dd/MM/yyyy format");
                    }
                    catch (Exception e)
                    {
                        if (e.InnerException is WebException)
                        {
                            HttpWebResponse resp = (HttpWebResponse)((WebException)e.InnerException).Response;
                            Console.WriteLine("Failed, {0}", resp.StatusDescription);
                        }
                        else
                        {
                            throw e;
                        }
                        
                    }
                }
            } while (true);
        }

        void search(string[] input)
        {
            if (input.Length != 4)
            {
                Console.WriteLine("Invalid parameters");
                Console.WriteLine("search <src> <dst> <dd/MM/yyyy>");
            }
            else
            {
                string src = input[1];
                string dst = input[2];
                string strDate = input[3];

                GetDate(strDate);

                Flights1 result = channel.GetFlights(src, dst, strDate);

                // TODO: sort data here ? we F*ck up the client
                foreach (Flight1 flight in result)
                {
                    Console.WriteLine("{0} {1} {2} seats {3}$", flight.name, flight.flightNumber, flight.seats, flight.price);
                }
            }
        }

        void reserve(string[] input)
        {
            if (input.Length != 4)
            {
                Console.WriteLine("Invalid parameters");
                Console.WriteLine("reserve <seller> <flight> <dd/MM/yyyy>");
            }
            else
            {
                FlightSearchServerCA.ReservationRequest reservationRequest = 
                    new FlightSearchServerCA.ReservationRequest();

                string seller = input[1];
                string flight = input[2];
                DateTime date = GetDate(input[3]);


                reservationRequest.date = date;
                reservationRequest.flightNumber = flight;

                int reservationId = channel.MakeReservation(seller, reservationRequest);
                Console.WriteLine("OK, reservation ID: {0}", reservationId);
            }
        }

        void cancel(string[] input) 
        {
            if (input.Length == 1) // I have no idea why this is required but I saw it in the excersize manual
            {
                Console.WriteLine("Invalid parameters");
                Console.WriteLine("cancel <seller> <reservation id>");
            }
            else
            {
                string seller = input[1];
                string reservationID = "";
                if (input.Length >= 3)
                {
                    reservationID = input[2];
                }

                channel.CancelReservation(seller, reservationID);
                Console.WriteLine("OK");                
            }
        }
    }
}
