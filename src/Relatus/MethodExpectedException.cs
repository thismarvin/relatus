using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Relatus
{
    internal class MethodExpectedException : Exception
    {
        public MethodExpectedException() : base("A particular method was expected to be called, but it appears it never was.")
        {
        }

        public MethodExpectedException(string message) : base(message)
        {
        }

        public MethodExpectedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MethodExpectedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
