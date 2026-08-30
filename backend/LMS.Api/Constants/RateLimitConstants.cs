namespace LMS.Api.Constants;

public static class RateLimitConstants
{
    public const string LoginPolicy = "LoginLimit";

    public const int LoginPermitLimit = 5;
    public const int LoginWindowMinutes = 1;
    public const int LoginQueueLimit = 0;
}