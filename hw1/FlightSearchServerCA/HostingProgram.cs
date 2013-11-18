using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace FlightSearchServerCA
{
    public class HostingProgram
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: ./server <clients port> <sellers port>");
                return;
            }
            
            try
            {
                Convert.ToInt32(args[0]);
                Convert.ToInt32(args[1]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid client or seller port: {0}",e.Message.ToString());
                return;
            }


            FlightSearchLogic fss = FlightSearchLogic.Instance; // DO NOT REMOVE THIS (THREAD CORRECTNESS)
            fss.Initialize(args[0], args[1]); // Host services
            fss.run(); // wait till death

            Console.ReadKey();
        }
    }
}