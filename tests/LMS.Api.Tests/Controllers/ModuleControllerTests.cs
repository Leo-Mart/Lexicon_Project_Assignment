using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Module;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMS.Api.Tests.Controllers;

public class ModuleControllerTests
{
    private readonly Mock<IModuleService> _moduleServiceMock;
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;

    public ModuleControllerTests()
    {
        _moduleServiceMock = new Mock<IModuleService>();
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
    }

    [Fact]
    public async Task GetModuleById_StudentRequestsModuleInOwnCourse_ReturnsModule()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = courseId,
            Name = "Test Module"
        };

        CourseDto studentCourse = new()
        {
            CourseId = courseId
        };

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        ModuleController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<ModuleDto> result =
            await controller.GetModuleById(moduleId);

        // Assert
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);
        Assert.Equal(moduleId, result.Value.ModuleId);
    }

    [Fact]
    public async Task GetModuleById_StudentRequestsModuleInAnotherCourse_ReturnsForbidden()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid studentCourseId = Guid.NewGuid();
        Guid otherCourseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = otherCourseId
        };

        CourseDto studentCourse = new()
        {
            CourseId = studentCourseId
        };

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        ModuleController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<ModuleDto> result =
            await controller.GetModuleById(moduleId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetModuleById_TeacherRequestsAnyModule_ReturnsModule()
    {
        // Arrange
        Guid teacherId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = courseId,
            Name = "Test Module"
        };

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        ModuleController controller =
            CreateController(teacherId, RoleConstants.Teacher);

        // Act
        ActionResult<ModuleDto> result =
            await controller.GetModuleById(moduleId);

        // Assert
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);
        Assert.Equal(moduleId, result.Value.ModuleId);

        _enrollmentServiceMock.Verify(
            service => service.GetStudentCourseAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ModuleController CreateController(
        Guid userId,
        string role)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        ];

        ClaimsIdentity identity =
            new(claims, "TestAuthentication");

        ClaimsPrincipal user = new(identity);

        return new ModuleController(
            _moduleServiceMock.Object,
            _enrollmentServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            }
        };
    }
}