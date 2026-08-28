using LMS.Api.Enums.Model;
using Microsoft.AspNetCore.Identity;

namespace LMS.Api.Models;

public class User : IdentityUser<Guid>
{
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Enrollment? Enrollment { get; set; }

    public ICollection<Resource> CreatedResources { get; set; } = new List<Resource>();

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();

    public ICollection<Submission> FeedbackSubmissions { get; set; } = new List<Submission>();
}
