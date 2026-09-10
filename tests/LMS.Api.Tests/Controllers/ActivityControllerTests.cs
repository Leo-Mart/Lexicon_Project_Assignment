using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Module;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMS.Api.Tests.Controllers;

public class ActivityControllerTests
{
    private readonly Mock<IActivityService> _activityServiceMock;
    private readonly Mock<IModuleService> _moduleServiceMock;
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;

    public ActivityControllerTests()
    {
        _activityServiceMock = new Mock<IActivityService>();
        _moduleServiceMock = new Mock<IModuleService>();
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
    }

    [Fact]
    public async Task GetActivityById_StudentRequestsActivityInOwnCourse_ReturnsOk()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();
        Guid activityId = Guid.NewGuid();

        ActivityDto activity = new()
        {
            ActivityId = activityId,
            ModuleId = moduleId
        };

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = courseId
        };

        CourseDto studentCourse = new()
        {
            CourseId = courseId
        };

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(
                activityId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        ActivityController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<ActivityDto?> result =
            await controller.GetActivityById(activityId);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        ActivityDto returnedActivity =
            Assert.IsType<ActivityDto>(okResult.Value);

        Assert.Equal(activityId, returnedActivity.ActivityId);
    }

    [Fact]
    public async Task GetActivityById_StudentRequestsActivityInAnotherCourse_ReturnsForbidden()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid studentCourseId = Guid.NewGuid();
        Guid otherCourseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();
        Guid activityId = Guid.NewGuid();

        ActivityDto activity = new()
        {
            ActivityId = activityId,
            ModuleId = moduleId
        };

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = otherCourseId
        };

        CourseDto studentCourse = new()
        {
            CourseId = studentCourseId
        };

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(
                activityId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        ActivityController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<ActivityDto?> result =
            await controller.GetActivityById(activityId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetActivityById_TeacherRequestsAnyActivity_ReturnsOk()
    {
        // Arrange
        Guid teacherId = Guid.NewGuid();
        Guid activityId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();

        ActivityDto activity = new()
        {
            ActivityId = activityId,
            ModuleId = moduleId
        };

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(
                activityId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        ActivityController controller =
            CreateController(teacherId, RoleConstants.Teacher);

        // Act
        ActionResult<ActivityDto?> result =
            await controller.GetActivityById(activityId);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        ActivityDto returnedActivity =
            Assert.IsType<ActivityDto>(okResult.Value);

        Assert.Equal(activityId, returnedActivity.ActivityId);

        _enrollmentServiceMock.Verify(
            service => service.GetStudentCourseAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _moduleServiceMock.Verify(
            service => service.GetModuleById(It.IsAny<Guid>()),
            Times.Never);
    }

    private ActivityController CreateController(
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

        return new ActivityController(
            _activityServiceMock.Object,
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
