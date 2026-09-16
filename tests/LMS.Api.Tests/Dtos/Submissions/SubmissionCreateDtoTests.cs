using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Submissions;

namespace LMS.Api.Tests.DTOs.Submissions;

public class SubmissionCreateDtoTests
{
    [Fact]
    public void SubmissionCreateDto_WithValidData_ShouldBeValid()
    {
        SubmissionCreateDto dto = new()
        {
            ActivityId = Guid.NewGuid(),
            Text = "Assignment handed in."
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void SubmissionCreateDto_WithoutText_ShouldBeInvalid()
    {
        SubmissionCreateDto dto = new()
        {
            ActivityId = Guid.NewGuid(),
            Text = string.Empty
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionCreateDto.Text)));
    }

    [Fact]
    public void SubmissionCreateDto_WithTextExceedingMaxLength_ShouldBeInvalid()
    {
        SubmissionCreateDto dto = new()
        {
            ActivityId = Guid.NewGuid(),
            Text = new string('a', 2501)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(SubmissionCreateDto.Text)));
    }

    [Fact]
    public void SubmissionCreateDto_WithTextAtMaxLength_ShouldBeValid()
    {
        SubmissionCreateDto dto = new()
        {
            ActivityId = Guid.NewGuid(),
            Text = new string('a', 2500)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(SubmissionCreateDto dto)
    {
        List<ValidationResult> results = [];

        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results;
    }
}
