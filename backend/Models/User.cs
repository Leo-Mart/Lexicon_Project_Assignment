using LFM.backend.Enum.Model;

namespace LMS.backend.Models;

public class User
{
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public UserType Role { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Enrollment? enrollment { get; set; }
    public ICollection<Resource> CreatedResource { get; set; } = new List<Resource>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public ICollection<Submission> FeedbackSubmissions { get; set; } = new List<Submission>();
}
