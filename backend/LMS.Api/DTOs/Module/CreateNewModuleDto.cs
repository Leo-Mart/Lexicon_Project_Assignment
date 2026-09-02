namespace LMS.Api.DTOs.Module;

public record CreateNewModuleDto
{
    public Guid CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
