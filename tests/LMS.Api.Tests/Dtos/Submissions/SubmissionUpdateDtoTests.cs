using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Submissions;

namespace LMS.Api.Tests.DTOs.Submissions;

public class SubmissionUpdateDtoTests
{
    [Fact]
    public void SubmissionUpdateDto_WithValidData_ShouldBeValid()
    {
        SubmissionUpdateDto dto = new() { Text = "Revised submission." };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void SubmissionUpdateDto_WithoutText_ShouldBeInvalid()
    {
        SubmissionUpdateDto dto = new() { Text = string.Empty };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionUpdateDto.Text)));
    }

    [Fact]
    public void SubmissionUpdateDto_WithTextExceedingMaxLength_ShouldBeInvalid()
    {
        SubmissionUpdateDto dto = new() { Text = new string('a', 2501) };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionUpdateDto.Text)));
    }

    [Fact]
    public void SubmissionUpdateDto_WithTextAtMaxLength_ShouldBeValid()
    {
        SubmissionUpdateDto dto = new() { Text = new string('a', 2500) };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(SubmissionUpdateDto dto)
    {
        List<ValidationResult> results = [];

        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results;
    }
}
