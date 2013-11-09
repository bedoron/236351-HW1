using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Web;
using TicketSellingServer;

namespace FlightSearchServerCA
{

    public class ClientQueryService : IClientQueryService
    {

        public Flights1 GetFlights(string src, string dst, string date)
        {
            Console.WriteLine("ClientQueryService: "+dst+" "+src+" "+date);
            Flights flights = null;
            try
            {
                flights = FlightSearchServer.Instance.QueryFlights(src, dst, date);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }
            Flights1 fs = new Flights1();
            foreach (Flight f in flights)
            {
                Flight1 f1 = new Flight1();
                f1.dst = f.dst;
                f1.src = f.src;
                f1.seats = f.seats;
                f1.name = f.name;
                f1.flightNumber = f.flightNumber;
                f1.date = f.date;
                fs.Add(f1);
            }
            return fs;
        }

        public int MakeReservation(string seller, ReservationRequest request)
        {
            int reservationID = 0;
            try
            {
                reservationID = FlightSearchServer.Instance.MakeReservation(seller, request);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }

            return reservationID;
        }
        public void CancelReservation(string seller, string CancelReservation)
        {
            try
            {
                FlightSearchServer.Instance.CancelReservation(seller, CancelReservation);
            }
            catch (Exception e)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound(e.Message);
            }

        }

    }
}