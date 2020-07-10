using System;
using System.Runtime.Serialization;

namespace Relatus
{
    [Serializable]
    internal class RelatusException : Exception
    {
        public RelatusException()
        {
        }

        public RelatusException(string message) : base("Something went wrong within the Mimoso Engine.")
        {
        }

        public RelatusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RelatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
