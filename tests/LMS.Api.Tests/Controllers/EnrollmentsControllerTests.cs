using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Enrollment;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMS.Api.Tests.Controllers;

public class EnrollmentsControllerTests
{
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;

    public EnrollmentsControllerTests()
    {
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
    }

    [Fact]
    public async Task GetCourseUsers_StudentRequestsOwnCourse_ReturnsOk()
    {
        // Arrange
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        CourseDto studentCourse = new()
        {
            CourseId = courseId
        };

        List<EnrollmentStudentsDto> students = [];

        _enrollmentServiceMock
            .Setup(service => service.GetStudentCourseAsync(
                studentId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(studentCourse);

        _enrollmentServiceMock
            .Setup(service => service.GetStudentEnrollmentsByCourseIdAsync(
                courseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        EnrollmentsController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<EnrollmentStudentsDto>> result =
            await controller.GetCourseUsers(
                courseId,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(students, okResult.Value);
    }

    [Fact]
    public async Task GetCourseUsers_StudentRequestsAnotherCourse_ReturnsForbidden()
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

        EnrollmentsController controller =
            CreateController(studentId, RoleConstants.Student);

        // Act
        ActionResult<List<EnrollmentStudentsDto>> result =
            await controller.GetCourseUsers(
                requestedCourseId,
                CancellationToken.None);

        // Assert
        Assert.IsType<ForbidResult>(result.Result);

        _enrollmentServiceMock.Verify(
            service => service.GetStudentEnrollmentsByCourseIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetCourseUsers_TeacherRequestsAnyCourse_ReturnsOk()
    {
        // Arrange
        Guid teacherId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        List<EnrollmentStudentsDto> students = [];

        _enrollmentServiceMock
            .Setup(service => service.GetStudentEnrollmentsByCourseIdAsync(
                courseId,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        EnrollmentsController controller =
            CreateController(teacherId, RoleConstants.Teacher);

        // Act
        ActionResult<List<EnrollmentStudentsDto>> result =
            await controller.GetCourseUsers(
                courseId,
                CancellationToken.None);

        // Assert
        OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(result.Result);

        Assert.Same(students, okResult.Value);

        _enrollmentServiceMock.Verify(
            service => service.GetStudentCourseAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private EnrollmentsController CreateController(
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

        return new EnrollmentsController(
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