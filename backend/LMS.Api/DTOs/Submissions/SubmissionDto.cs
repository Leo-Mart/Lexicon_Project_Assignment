namespace LMS.Api.DTOs.Submissions;

using LMS.Api.Enums.Model;
using LMS.Api.Models;

public class SubmissionDto
{
    public Guid SubmissionId { get; set; }
    public Guid ActivityId { get; set; }
    public Guid StudentId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public SubmissionStatus Status { get; set; }
    public string? Feedback { get; set; }
    public Guid? FeedbackByTeacherId { get; set; }
    public DateTime? FeedbackAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
