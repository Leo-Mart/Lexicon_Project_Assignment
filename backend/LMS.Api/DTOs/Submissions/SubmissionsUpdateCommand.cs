namespace LMS.Api.DTOs.Submissions;

public record SubmissionsUpdateCommand
{
    public required Guid SubmissionId { get; init; }
    public required Guid StudentId { get; init; }
    public required string Text { get; init; }
}
