using System;
using System.Runtime.Serialization;

namespace Mimoso
{
    [Serializable]
    internal class MimosoException : Exception
    {
        public MimosoException()
        {
        }

        public MimosoException(string message) : base("Something went wrong within the Mimoso Engine.")
        {
        }

        public MimosoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MimosoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
