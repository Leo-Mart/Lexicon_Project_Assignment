using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs.Resources;

public class ResourceUpdateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? Content { get; set; }

    [Url]
    public string? Uri { get; set; }
}
