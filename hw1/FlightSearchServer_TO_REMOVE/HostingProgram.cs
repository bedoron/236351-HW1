using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace FlightSearchServer
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(
                typeof(ClientQueryService), new Uri("http://localhost:8080/Services")))
            {
                host.Open();
                Console.ReadKey();
            }
        }
    }
}