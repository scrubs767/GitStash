using System;
using System.Runtime.Serialization;

namespace GitWrapper
{
    [Serializable]
    public class GitStashException : Exception
    {
        private Exception ex;

        public GitStashException()
        {
        }

        public GitStashException(string message) : base(message)
        {
        }

        public GitStashException(Exception ex)
        {
            this.ex = ex;
        }

        public GitStashException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GitStashException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}