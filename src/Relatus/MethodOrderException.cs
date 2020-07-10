using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Relatus
{
    internal class MethodOrderException : Exception
    {
        public MethodOrderException() : base("A method was called in the incorrect order. A particular method may have been required before calling this method.")
        {
        }

        public MethodOrderException(string message) : base(message)
        {
        }

        public MethodOrderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MethodOrderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
