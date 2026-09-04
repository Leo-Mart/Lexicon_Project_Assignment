namespace LMS.Api.DTOs.Resources;

public class ResourceDto
{
    public Guid ResourceId { get; set; }

    public Guid CreatedByTeacherId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string? Uri { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
