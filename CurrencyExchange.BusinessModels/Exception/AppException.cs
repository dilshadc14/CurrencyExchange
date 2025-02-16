
using System;
using System.Net;

namespace CurrencyExchange.BusinessModels
{
   
    public class ApiException : Exception
    {
        public HttpStatusCode StatusCode { get; }
        
        public ApiException(HttpStatusCode statusCode, string message) :base(message)
        {
            StatusCode = statusCode;
           
        }
    }
}
