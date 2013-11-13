using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TicketSellingServer
{
    /// <summary>
    /// Adapts the service with the logic
    /// </summary>
    class TicketSellingQueryService: ITicketSellingQueryService
    {

        public Flights GetFlights(FlightQuery flightQuery)
        {
            Flights fs;
            try
            {
                fs = TicketSellingQueryLogic.Instance.GetFlights(flightQuery);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return fs;
        }

        public int MakeReservation(FlightSearchReservationRequest request)
        {
            int resID;
            try
            {
                resID = TicketSellingQueryLogic.Instance.MakeReservation(request);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            return resID;
        }

        public void CancelReservation(int reservationID)
        {
            try
            {
                TicketSellingQueryLogic.Instance.CancelReservation(reservationID);
            }
            catch (Exception e)
            {
                throw new FaultException(e.Message);
            }
            
        }
    }
}
