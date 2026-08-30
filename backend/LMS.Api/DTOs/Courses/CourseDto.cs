using LMS.Api.Models;

namespace LMS.Api.DTOs.Courses
{
    public record CourseDto
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }
}
