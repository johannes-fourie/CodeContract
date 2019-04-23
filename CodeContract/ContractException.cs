using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CodeContract
{
    class ContractViolationException : Exception
    {
        public ContractViolationException()
        {
        }

        public ContractViolationException(string message) : base(message)
        {
        }

        public ContractViolationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContractViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
