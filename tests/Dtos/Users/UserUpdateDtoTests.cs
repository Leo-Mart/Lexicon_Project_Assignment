using System.ComponentModel.DataAnnotations;


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
            Status = UserStatus.Active
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Empty(results);
    }

    [Fact]
    public void UserUpdateDto_WithNullNameAndEmail_IsValid()
    {
        var dto = new UserUpdateDto
        {
            Name = null,
            Email = null,
            Status = UserStatus.Active
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
            Status = UserStatus.Active
        };

        List<ValidationResult> results = Validate(dto);

        Assert.Contains(
            results,
            result => result.MemberNames.Contains(nameof(UserUpdateDto.Email))
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