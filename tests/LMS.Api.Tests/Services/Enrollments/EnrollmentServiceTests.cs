using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Course;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using Microsoft.AspNetCore.Identity;
using Moq;
using CourseModel = LMS.Api.Models.Course;

namespace LMS.Api.Tests.Services.Enrollments;

public class EnrollmentServiceTests
{
    private readonly Mock<IEnrollmentRepository> _enrollmentRepositoryMock;
    private readonly Mock<ICourseRepository> _courseRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly EnrollmentService _enrollmentService;

    public EnrollmentServiceTests()
    {
        _enrollmentRepositoryMock = new Mock<IEnrollmentRepository>();
        _courseRepositoryMock = new Mock<ICourseRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userManagerMock = CreateUserManagerMock();
        _mapperMock = new Mock<IMapper>();

        _enrollmentService = new EnrollmentService(
            _enrollmentRepositoryMock.Object,
            _courseRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object
        );
    }

    private static Mock<UserManager<User>> CreateUserManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<User>>();

        return new Mock<UserManager<User>>(
            userStoreMock.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );
    }
    [Fact]
    public async Task GetStudentCourseAsync_WhenEnrollmentExists_ReturnsCourse()
    {
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var course = new CourseModel
        {
            CourseId = courseId,
            Name = "C# Development"
        };

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow,
            Course = course
        };

        var courseDto = new CourseDto
        {
            CourseId = courseId,
            Name = "C# Development"
        };

        _enrollmentRepositoryMock
            .Setup(repository =>
                repository.GetByStudentIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        _mapperMock
            .Setup(mapper => mapper.Map<CourseDto>(course))
            .Returns(courseDto);

        CourseDto? result =
            await _enrollmentService.GetStudentCourseAsync(studentId);

        Assert.NotNull(result);
        Assert.Equal(courseId, result.CourseId);
        Assert.Equal("C# Development", result.Name);
    }
    [Fact]
    public async Task GetStudentCourseAsync_WhenEnrollmentDoesNotExist_ReturnsNull()
    {
        Guid studentId = Guid.NewGuid();

        _enrollmentRepositoryMock
            .Setup(repository =>
                repository.GetByStudentIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enrollment?)null);

        CourseDto? result =
            await _enrollmentService.GetStudentCourseAsync(studentId);

        Assert.Null(result);

        _mapperMock.Verify(
            mapper => mapper.Map<CourseDto>(It.IsAny<LMS.Api.Models.Course>()),
            Times.Never
        );
    }
}
