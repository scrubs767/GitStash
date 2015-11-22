using System;
using System.Runtime.Serialization;

namespace GitWrapper
{
    [Serializable]
    public class GitStashInvalidIndexException : Exception
    {
        public GitStashInvalidIndexException()
        {
        }

        public GitStashInvalidIndexException(string message) : base(message)
        {
        }

        public GitStashInvalidIndexException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GitStashInvalidIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}