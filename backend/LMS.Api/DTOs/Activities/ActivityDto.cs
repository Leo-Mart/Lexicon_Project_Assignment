using LMS.Api.Enums.Model;

namespace LMS.Api.DTOs.Activities;

public class ActivityDto
{
    public Guid ActivityId { get; set; }

    public Guid ModuleId { get; set; }

    public ActivityType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? Deadline { get; set; }
}
