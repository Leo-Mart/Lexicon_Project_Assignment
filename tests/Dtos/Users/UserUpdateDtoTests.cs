using System.ComponentModel.DataAnnotations;
using LMS.Api.DTOs.Users;
using LMS.Api.Enums.Model;

namespace LMS.Api.Tests.DTOs.Users;

public class UserUpdateDtoTests
{
    [Fact]
    public void UserUpdateDto_WithValidData_IsValid()
    {
        var dto = new UserUpdateDto
        {
            Name = "Updated User",
            Email = "updated@example.com",
            Status = UserStatus.Active,
            Role = "Student"
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void UserUpdateDto_WithNullOptionalValues_IsValid()
    {
        var dto = new UserUpdateDto
        {
            Name = null,
            Email = null,
            Status = UserStatus.Active,
            Role = null
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void UserUpdateDto_WithInvalidEmail_IsInvalid()
    {
        var dto = new UserUpdateDto
        {
            Name = "Updated User",
            Email = "invalid-email",
            Status = UserStatus.Active,
            Role = "Student"
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains(nameof(UserUpdateDto.Email))
        );
    }

    [Theory]
    [InlineData("Student")]
    [InlineData("Teacher")]
    public void UserUpdateDto_WithValidRole_IsValid(string role)
    {
        var dto = new UserUpdateDto
        {
            Status = UserStatus.Active,
            Role = role
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("Admin")]
    [InlineData("TeacherAdmin")]
    [InlineData("student")]
    [InlineData("Invalid")]
    public void UserUpdateDto_WithInvalidRole_IsInvalid(string role)
    {
        var dto = new UserUpdateDto
        {
            Status = UserStatus.Active,
            Role = role
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains(nameof(UserUpdateDto.Role))
        );
    }

    private static List<ValidationResult> Validate(UserUpdateDto dto)
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