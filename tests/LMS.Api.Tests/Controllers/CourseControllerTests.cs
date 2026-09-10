using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Course;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMS.Api.Tests.Controllers;

public class CourseControllerTests
{
    private readonly Mock<ICourseService> _courseServiceMock;
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;

    public CourseControllerTests()
    {
        _courseServiceMock = new Mock<ICourseService>();
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
    }

    [Fact]
    public async Task GetCourseById_StudentRequestsOwnCourse_ReturnsCourse()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        CourseDto course = new()
        {
            CourseId = courseId,
            Name = "Test Course"
        };

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(course);

        _courseServiceMock
            .Setup(service => service.GetCourseById(courseId))
            .ReturnsAsync(course);

        CourseController controller = CreateController(
            studentId,
            RoleConstants.Student);

        // Act
        ActionResult<CourseDto> result =
            await controller.GetCourseById(courseId);

        // Assert
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);
        Assert.Equal(courseId, result.Value.CourseId);
    }

    [Fact]
    public async Task GetCourseById_StudentRequestsAnotherCourse_ReturnsForbidden()
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

        CourseController controller = CreateController(
            studentId,
            RoleConstants.Student);

        // Act
        ActionResult<CourseDto> result =
            await controller.GetCourseById(requestedCourseId);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async Task GetCourseById_TeacherRequestsAnyCourse_ReturnsCourse()
    {
        // Arrange
        Guid teacherId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        CourseDto course = new()
        {
            CourseId = courseId,
            Name = "Test Course"
        };

        _courseServiceMock
            .Setup(service => service.GetCourseById(courseId))
            .ReturnsAsync(course);

        CourseController controller = CreateController(
            teacherId,
            RoleConstants.Teacher);

        // Act
        ActionResult<CourseDto> result =
            await controller.GetCourseById(courseId);

        // Assert
        Assert.Null(result.Result);
        Assert.NotNull(result.Value);
        Assert.Equal(courseId, result.Value.CourseId);

        _enrollmentServiceMock.Verify(
            service => service.GetStudentCourseAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private CourseController CreateController(
        Guid userId,
        string role)
    {
        Claim[] claims =
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        ];

        ClaimsIdentity identity = new(claims, "TestAuthentication");
        ClaimsPrincipal user = new(identity);

        return new CourseController(
            _courseServiceMock.Object,
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