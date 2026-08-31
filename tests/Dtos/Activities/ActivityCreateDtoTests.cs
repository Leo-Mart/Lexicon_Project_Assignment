using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Activities;
using LMS.Api.Enums.Model;

namespace LMS.Api.Tests.DTOs.Activities;

public class ActivityCreateDtoTests
{
    [Fact]
    public void ActivityCreateDto_WithValidData_ShouldBeValid()
    {
        ActivityCreateDto dto = new()
        {
            ModuleId = Guid.NewGuid(),
            Type = ActivityType.Lecture,
            Name = "Introduction",
            Description = "Introduction to the module.",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2),
            Deadline = null
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void ActivityCreateDto_WithoutName_ShouldBeInvalid()
    {
        ActivityCreateDto dto = new()
        {
            ModuleId = Guid.NewGuid(),
            Type = ActivityType.Lecture,
            Name = string.Empty,
            Description = "Introduction to the module.",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ActivityCreateDto.Name)));
    }

    [Fact]
    public void ActivityCreateDto_WithoutDescription_ShouldBeInvalid()
    {
        ActivityCreateDto dto = new()
        {
            ModuleId = Guid.NewGuid(),
            Type = ActivityType.Lecture,
            Name = "Introduction",
            Description = string.Empty,
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ActivityCreateDto.Description)));
    }

    [Fact]
    public void ActivityCreateDto_WithInvalidActivityType_ShouldBeInvalid()
    {
        ActivityCreateDto dto = new()
        {
            ModuleId = Guid.NewGuid(),
            Type = (ActivityType)999,
            Name = "Introduction",
            Description = "Introduction to the module.",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ActivityCreateDto.Type)));
    }

    [Fact]
    public void ActivityCreateDto_WithoutDeadline_ShouldBeValid()
    {
        ActivityCreateDto dto = new()
        {
            ModuleId = Guid.NewGuid(),
            Type = ActivityType.Lecture,
            Name = "Introduction",
            Description = "Introduction to the module.",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2),
            Deadline = null
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(ActivityCreateDto dto)
    {
        List<ValidationResult> results = [];

        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);

        return results;
    }
}