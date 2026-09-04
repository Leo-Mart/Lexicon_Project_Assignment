using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Course;

namespace LMS.Api.DTOs.Module;

public record ModuleDto
{
    public Guid ModuleId { get; set; }
    public Guid CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public BasicCourseInfoDto Course { get; set; } = null!;
    public ICollection<ActivityDto> Activities { get; set; } = new List<ActivityDto>();
}
