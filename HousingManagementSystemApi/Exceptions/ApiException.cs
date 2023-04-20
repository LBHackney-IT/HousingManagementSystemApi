using System;
using System.Net;

namespace HousingManagementSystemApi.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(HttpStatusCode statusCode)
            : base($"API request failed with statusCode {statusCode}")
        {

        }
    }
}
