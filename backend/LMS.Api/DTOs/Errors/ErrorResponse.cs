namespace LMS.Api.DTOs.Errors;

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int? StatusCode { get; set; }
}
