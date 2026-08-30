using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Users;

namespace LMS.Api.Tests.DTOs.Users;

public class UserCreateDtoTests
{
    [Fact]
    public void UserCreateDto_WithValidData_IsValid()
    {
        var dto = new UserCreateDto
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password123!",
            Role = "Student"
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void UserCreateDto_WithInvalidEmail_IsInvalid()
    {
        var dto = new UserCreateDto
        {
            Name = "Test User",
            Email = "invalid-email",
            Password = "Password123!",
            Role = "Student"
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains(nameof(UserCreateDto.Email))
        );
    }

    [Fact]
    public void UserCreateDto_WithoutName_IsInvalid()
    {
        var dto = new UserCreateDto
        {
            Name = string.Empty,
            Email = "test@example.com",
            Password = "Password123!",
            Role = "Student"
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains(nameof(UserCreateDto.Name))
        );
    }

    private static List<ValidationResult> Validate(UserCreateDto dto)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(dto);

        Validator.TryValidateObject(
            dto,
            context,
            results,
            validateAllProperties: true
        );

        return results;
    }
}