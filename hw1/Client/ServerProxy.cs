using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Globalization;
using System.ServiceModel;

namespace Client
{
    class ServerProxy
    {
        WebChannelFactory<FlightSearchServerCA.IClientQueryService>
            cf;

        Uri serverUri;
        FlightSearchServerCA.IClientQueryService channel;

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
                        if (command.Equals("search"))
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
                try
                {
                    DateTime date = DateTime.ParseExact(strDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                } 
                catch(FormatException) 
                {
                    Console.WriteLine("Invalid time format, use dd/MM/yyyy format");
                    return;
                }
                FlightSearchServerCA.Flights result;
                try
                {
                    result = channel.GetFlights(src, dst, strDate);
                } catch(WebFaultException e) {
                    Console.WriteLine("Service failed: {0}", e.Reason);
                    return;
                }

                // TODO: sort data here ? we F*ck up the client
                foreach (FlightSearchServerCA.Flight flight in result)
                {
                    // TODO: print by spec
                    Console.WriteLine("{0} {1} {2} seats {3}$", flight.name, flight.flightNumber, flight.seats, flight.price);
                }
            }
        }

        void reserve(string[] input)
        {
            Console.WriteLine("Reserve");
        }

        void cancel(string[] input) 
        {
            Console.WriteLine("Cancel");
        }
    }
}
