using LMS.Api.Enums.Model;
namespace LMS.Api.Models;

public class Submission
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

    public Activity Activity { get; set; } = null!;
    public User Student { get; set; } = null!;
    public User? FeedbackByTeacher { get; set; }
}
