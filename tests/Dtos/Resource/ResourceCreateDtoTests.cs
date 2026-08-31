using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Resources;

namespace LMS.Api.Tests.DTOs.Resources;

public class ResourceCreateDtoTests
{
    [Fact]
    public void ResourceCreateDto_WithValidData_ShouldBeValid()
    {
        ResourceCreateDto dto = new()
        {
            Name = "Course documentation",
            Description = "Documentation for the course.",
            Uri = "https://example.com/documentation"
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void ResourceCreateDto_WithoutName_ShouldBeInvalid()
    {
        ResourceCreateDto dto = new()
        {
            Name = string.Empty,
            Description = "Documentation for the course."
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ResourceCreateDto.Name)));
    }

    [Fact]
    public void ResourceCreateDto_WithoutUri_ShouldBeValid()
    {
        ResourceCreateDto dto = new()
        {
            Name = "Course documentation",
            Description = "Documentation for the course.",
            Uri = null
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    private static List<ValidationResult> Validate(ResourceCreateDto dto)
    {
        List<ValidationResult> results = [];

        Validator.TryValidateObject(
            dto,
            new ValidationContext(dto),
            results,
            validateAllProperties: true
        );

        return results;
    }
}