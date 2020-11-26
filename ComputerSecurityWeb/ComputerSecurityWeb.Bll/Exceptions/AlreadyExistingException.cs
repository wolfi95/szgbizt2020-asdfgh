using System;

namespace ComputerSecurityWeb.Bll.Exceptions
{
    public class AlreadyExistingException : Exception
    {
        public AlreadyExistingException()
        {

        }

        public AlreadyExistingException(string message)
           : base(message)
        {
        }

        public AlreadyExistingException(string message, Exception innerException)
           : base(message, innerException)
        {
        }
    }
}
