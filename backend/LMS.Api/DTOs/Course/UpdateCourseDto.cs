using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs.Course;

public record UpdateCourseDto
{
    [Required(ErrorMessage = "A course name is required")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "A course description is required")]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "A course start-date is required")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "A course end-date is required")]
    public DateOnly EndDate { get; set; }
}
