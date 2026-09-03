namespace LMS.Api.DTOs.Submissions;

public record SubmissionsCreateCommand
{
    public required Guid StudentId;
    public required Guid ActivityId;
    public required string Text;
}
