namespace LMS.Api.Exceptions;

public class InvalidActivityTypeException(string message, int statusCode) : Exception(message)
{
    public int StatusCode = statusCode;
}

// Resubmission is only allowed while a teacher has flagged it as needing completion.
public class InvalidSubmissionStateException(string message, int statusCode) : Exception(message)
{
    public int StatusCode = statusCode;
}
