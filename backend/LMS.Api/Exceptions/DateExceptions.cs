namespace LMS.Api.Exceptions
{
    public class InvalidDateException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode = statusCode;
    }

    public class OverlappingDateException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode = statusCode;
    }
}
