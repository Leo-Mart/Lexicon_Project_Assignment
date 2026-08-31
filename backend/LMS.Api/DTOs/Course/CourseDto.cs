using LMS.Api.DTOs.Module;

namespace LMS.Api.DTOs.Course
{
    public record CourseDto
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public ICollection<ModuleDto> Modules { get; set; } = new List<ModuleDto>();
    }
}
