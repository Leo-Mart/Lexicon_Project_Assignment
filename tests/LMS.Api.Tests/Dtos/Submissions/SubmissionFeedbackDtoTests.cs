using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;

namespace LMS.Api.Tests.DTOs.Submissions;

public class SubmissionFeedbackDtoTests
{
    [Fact]
    public void SubmissionFeedbackDto_WithValidData_ShouldBeValid()
    {
        SubmissionFeedbackDto dto = new()
        {
            Feedback = "Good work.",
            ReviewStatus = SubmissionReviewStatus.Approved
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void SubmissionFeedbackDto_WithoutFeedback_ShouldBeInvalid()
    {
        SubmissionFeedbackDto dto = new()
        {
            Feedback = string.Empty,
            ReviewStatus = SubmissionReviewStatus.Approved
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionFeedbackDto.Feedback)));
    }

    [Fact]
    public void SubmissionFeedbackDto_WithFeedbackTooShort_ShouldBeInvalid()
    {
        SubmissionFeedbackDto dto = new()
        {
            Feedback = "Hi",
            ReviewStatus = SubmissionReviewStatus.Approved
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionFeedbackDto.Feedback)));
    }

    [Fact]
    public void SubmissionFeedbackDto_WithFeedbackExceedingMaxLength_ShouldBeInvalid()
    {
        SubmissionFeedbackDto dto = new()
        {
            Feedback = new string('a', 2001),
            ReviewStatus = SubmissionReviewStatus.Approved
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionFeedbackDto.Feedback)));
    }

    private static List<ValidationResult> Validate(SubmissionFeedbackDto dto)
    {
        List<ValidationResult> results = [];

        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results;
    }
}
