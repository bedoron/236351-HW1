using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightSearchServerCA 
{
    [Serializable]
    public class FlightSearchServerException : Exception
    {
        public FlightSearchServerException()
        : base() { }
    
        public FlightSearchServerException(string message)
        : base(message) { }
    
        public FlightSearchServerException(string format, params object[] args)
        : base(string.Format(format, args)) { }
    
        public FlightSearchServerException(string message, Exception innerException)
        : base(message, innerException) { }
    
        public FlightSearchServerException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }

        protected FlightSearchServerException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }

    }
}
