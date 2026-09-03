using System.ComponentModel.DataAnnotations;

namespace LMS.Api.DTOs.Submissions
{
    public record SubmissionCreateDto
    {
        [Required(ErrorMessage = "An activity id is required.")]
        public Guid ActivityId { get; set; }

        [Required(ErrorMessage = "A submission text is required ")]
        [MaxLength(2500)]
        public string Text { get; set; } = string.Empty;
    }
}
