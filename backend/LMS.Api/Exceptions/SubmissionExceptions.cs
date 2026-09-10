namespace LMS.Api.Exceptions;

public class InvalidActivityTypeException(string message, int statusCode) : Exception(message)
{
    public int StatusCode = statusCode;
}
