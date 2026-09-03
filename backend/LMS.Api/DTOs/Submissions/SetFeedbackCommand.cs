namespace LMS.Api.DTOs.Submissions;

public record SetFeedbackCommand
{
    public required Guid SubmissionId { get; init; }
    public required Guid TeacherId { get; init; }
    public required SubmissionFeedbackDto Details { get; init; }
}
