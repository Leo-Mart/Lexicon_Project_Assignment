namespace LMS.Api.DTOs.Submissions;

// An enrolled student with no submission, past the activity's deadline -
// not a real Submission, so it doesn't share SubmissionDto's shape.
public class OverdueSubmissionDto
{
    public Guid ActivityId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
}
