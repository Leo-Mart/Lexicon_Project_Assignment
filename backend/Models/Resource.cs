namespace LFS.backend.Models

public class Resource
{
    public Guid ResourceId { get; set; }
    public Guid CreatedByTeacherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Uri { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User CreatedByTeacher { get; set; } = null!;
    public ICollection<CourseResource> CourseResources { get; set; } = new List<CourseResource>();
    public ICollection<ModuleResource> ModuleResources { get; set; } = new List<ModuleResource>();
    public ICollection<ActivityResource> ActivityResources { get; set; } = new List<ActivityResource>();
}