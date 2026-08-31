using System.ComponentModel.DataAnnotations;
using LMS.Api.Enums.Model;

namespace LMS.Api.DTOs.Activities;

public class ActivityUpdateDto
{
    [EnumDataType(typeof(ActivityType))]
    public ActivityType Type { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public DateTime? Deadline { get; set; }
}