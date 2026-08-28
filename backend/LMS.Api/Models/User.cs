using LMS.Api.Enums.Model;

namespace LMS.Api.Models;

public class User
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserType Role { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Enrollment? Enrollment { get; set; }

    public ICollection<Resource> CreatedResources { get; set; } = new List<Resource>();

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public ICollection<Submission> FeedbackSubmissions { get; set; } = new List<Submission>();
}
