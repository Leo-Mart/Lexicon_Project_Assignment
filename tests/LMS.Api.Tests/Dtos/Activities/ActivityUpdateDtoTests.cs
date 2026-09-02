using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Activities;
using LMS.Api.Enums.Model;

namespace LMS.Api.Tests.DTOs.Activities;

public class ActivityUpdateDtoTests
{
    [Fact]
    public void ActivityUpdateDto_WithValidData_ShouldBeValid()
    {
        ActivityUpdateDto dto = new()
        {
            Type = ActivityType.Lecture,
            Name = "Updated activity",
            Description = "Updated description.",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2),
            Deadline = null
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void ActivityUpdateDto_WithoutName_ShouldBeInvalid()
    {
        ActivityUpdateDto dto = new()
        {
            Type = ActivityType.Lecture,
            Name = string.Empty,
            Description = "Description",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ActivityUpdateDto.Name)));
    }

    [Fact]
    public void ActivityUpdateDto_WithoutDescription_ShouldBeInvalid()
    {
        ActivityUpdateDto dto = new()
        {
            Type = ActivityType.Lecture,
            Name = "Activity",
            Description = string.Empty,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ActivityUpdateDto.Description)));
    }

    [Fact]
    public void ActivityUpdateDto_WithInvalidActivityType_ShouldBeInvalid()
    {
        ActivityUpdateDto dto = new()
        {
            Type = (ActivityType)999,
            Name = "Activity",
            Description = "Description",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ActivityUpdateDto.Type)));
    }

    private static List<ValidationResult> Validate(ActivityUpdateDto dto)
    {
        List<ValidationResult> results = [];

        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results;
    }
}
