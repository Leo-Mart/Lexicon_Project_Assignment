using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs.Submissions
{
    public record SubmissionUpdateDto
    {
        [Required(ErrorMessage = "A submission text is required ")]
        [MaxLength(2500)]
        public string Text { get; set; } = string.Empty;
    }
}
