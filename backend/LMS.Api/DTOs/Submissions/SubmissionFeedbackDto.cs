using System.ComponentModel.DataAnnotations;
using LMS.Api.Constants;
namespace LMS.Api.DTOs.Submissions;

public class SubmissionFeedbackDto
{
    [MinLength(3)]
    [MaxLength(ModelConstants.DescriptionMaxLength)]
    [Required] public string Feedback { get; set; } = string.Empty;

}
