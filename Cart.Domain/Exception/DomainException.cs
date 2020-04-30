﻿﻿namespace Cart.Domain.Exception
{
    public class DomainException : System.Exception
    {
        public DomainException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}