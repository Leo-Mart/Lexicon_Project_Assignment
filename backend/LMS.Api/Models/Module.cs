namespace LMS.Api.Models;

public class Module
{
    public Guid ModuleId { get; set; }
    public Guid CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Course Course { get; set; } = null!;
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<ModuleResource> ModuleResources { get; set; } = new List<ModuleResource>();
}
