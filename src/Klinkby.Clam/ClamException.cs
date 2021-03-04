using System;

namespace Klinkby.Clam
{
    public class ClamException : Exception
    {
        public ClamException() { }
        public ClamException(string message) : base(message) { }
        public ClamException(string message, Exception inner) : base(message, inner) { }
        protected ClamException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
