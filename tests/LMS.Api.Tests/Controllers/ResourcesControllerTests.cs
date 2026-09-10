using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Module;
using LMS.Api.DTOs.Resources;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMS.Api.Tests.Controllers;

public class ResourcesControllerTests
{
    private readonly Mock<IResourceService> _resourceServiceMock;
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
    private readonly Mock<IModuleService> _moduleServiceMock;
    private readonly Mock<IActivityService> _activityServiceMock;

    public ResourcesControllerTests()
    {
        _resourceServiceMock = new Mock<IResourceService>();
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
        _moduleServiceMock = new Mock<IModuleService>();
        _activityServiceMock = new Mock<IActivityService>();
    }

    [Fact]
    public async Task GetByCourseId_StudentRequestsOwnCourse_ReturnsOk()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = courseId
        };

        List<ResourceDto> resources = [];

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        _resourceServiceMock
            .Setup(service => service.GetByCourseIdAsync(
                courseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resources);

        ResourcesController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByCourseId(
                courseId,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(resources, okResult.Value);
    }

    [Fact]
    public async Task GetByCourseId_StudentRequestsAnotherCourse_ReturnsForbidden()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid studentCourseId = Guid.NewGuid();
        Guid requestedCourseId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = studentCourseId
        };

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        ResourcesController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByCourseId(
                requestedCourseId,
                CancellationToken.None);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);

        _resourceServiceMock.Verify(
            service => service.GetByCourseIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByCourseId_TeacherRequestsAnyCourse_ReturnsOk()
    {
        // Arrange
        Guid teacherId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        List<ResourceDto> resources = [];

        _resourceServiceMock
            .Setup(service => service.GetByCourseIdAsync(
                courseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resources);

        ResourcesController controller =
            CreateController(teacherId, RoleConstants.Teacher);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByCourseId(
                courseId,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(resources, okResult.Value);

        _enrollmentServiceMock.Verify(
            service => service.GetStudentCourseAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private ResourcesController CreateController(
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

        return new ResourcesController(
            _resourceServiceMock.Object,
            _enrollmentServiceMock.Object,
            _moduleServiceMock.Object,
            _activityServiceMock.Object)
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
    [Fact]
    public async Task GetByModuleId_StudentRequestsModuleInOwnCourse_ReturnsOk()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = courseId
        };

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = courseId
        };

        List<ResourceDto> resources = [];

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        _resourceServiceMock
            .Setup(service => service.GetByModuleIdAsync(
                moduleId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resources);

        ResourcesController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByModuleId(
                moduleId,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(resources, okResult.Value);
    }
    [Fact]
    public async Task GetByModuleId_StudentRequestsModuleInAnotherCourse_ReturnsForbidden()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid studentCourseId = Guid.NewGuid();
        Guid otherCourseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = studentCourseId
        };

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = otherCourseId
        };

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        ResourcesController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByModuleId(
                moduleId,
                CancellationToken.None);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);

        _resourceServiceMock.Verify(
            service => service.GetByModuleIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByActivityId_StudentRequestsActivityInOwnCourse_ReturnsOk()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();
        Guid activityId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = courseId
        };

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = courseId
        };

        ActivityDto activity = new()
        {
            ActivityId = activityId,
            ModuleId = moduleId
        };

        List<ResourceDto> resources = [];

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(
                activityId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        _resourceServiceMock
            .Setup(service => service.GetByActivityIdAsync(
                activityId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(resources);

        ResourcesController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByActivityId(
                activityId,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(resources, okResult.Value);
    }

    [Fact]
    public async Task GetByActivityId_StudentRequestsActivityInAnotherCourse_ReturnsForbidden()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid studentCourseId = Guid.NewGuid();
        Guid otherCourseId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();
        Guid activityId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = studentCourseId
        };

        ModuleDto module = new()
        {
            ModuleId = moduleId,
            CourseId = otherCourseId
        };

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

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        _moduleServiceMock
            .Setup(service => service.GetModuleById(moduleId))
            .ReturnsAsync(module);

        ResourcesController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<ResourceDto>> result =
            await controller.GetByActivityId(
                activityId,
                CancellationToken.None);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);

        _resourceServiceMock.Verify(
            service => service.GetByActivityIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}