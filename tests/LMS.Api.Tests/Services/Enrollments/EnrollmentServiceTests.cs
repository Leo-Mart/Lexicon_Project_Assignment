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

        CourseDto? result = await _enrollmentService.GetStudentCourseAsync(studentId);

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

        CourseDto? result = await _enrollmentService.GetStudentCourseAsync(studentId);

        Assert.Null(result);

        _mapperMock.Verify(
            mapper => mapper.Map<CourseDto>(It.IsAny<LMS.Api.Models.Course>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AssignOrChangeCourseAsync_WhenStudentDoesNotExist_ThrowsKeyNotFoundException()
    {
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(studentId.ToString()))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _enrollmentService.AssignOrChangeCourseAsync(
                studentId,
                courseId
            )
        );

        _courseRepositoryMock.Verify(
            repository => repository.GetCourseByIdAsync(It.IsAny<Guid>()),
            Times.Never
        );

        _enrollmentRepositoryMock.Verify(
            repository => repository.GetByStudentIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
    [Fact]
    public async Task AssignOrChangeCourseAsync_WhenUserIsNotStudent_ThrowsInvalidOperationException()
    {
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var user = new User
        {
            Id = studentId,
            Name = "Test User",
            Email = "test@example.com"
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(studentId.ToString()))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Teacher" });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _enrollmentService.AssignOrChangeCourseAsync(
                studentId,
                courseId
            )
        );

        _courseRepositoryMock.Verify(
            repository => repository.GetCourseByIdAsync(It.IsAny<Guid>()),
            Times.Never
        );

        _enrollmentRepositoryMock.Verify(
            repository => repository.GetByStudentIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task AssignOrChangeCourseAsync_WhenCourseDoesNotExist_ThrowsKeyNotFoundException()
    {
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var student = new User
        {
            Id = studentId,
            Name = "Test Student",
            Email = "student@example.com"
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(studentId.ToString()))
            .ReturnsAsync(student);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(student))
            .ReturnsAsync(new List<string> { "Student" });

        _courseRepositoryMock
            .Setup(repository => repository.GetCourseByIdAsync(courseId))
            .ReturnsAsync((LMS.Api.Models.Course?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _enrollmentService.AssignOrChangeCourseAsync(
                studentId,
                courseId
            )
        );

        _enrollmentRepositoryMock.Verify(
            repository => repository.GetByStudentIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
    [Fact]
    public async Task AssignOrChangeCourseAsync_WhenEnrollmentDoesNotExist_CreatesEnrollment()
    {
        Guid studentId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var student = new User
        {
            Id = studentId,
            Name = "Test Student",
            Email = "student@example.com"
        };

        var course = new LMS.Api.Models.Course
        {
            CourseId = courseId,
            Name = "C# Development"
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(studentId.ToString()))
            .ReturnsAsync(student);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(student))
            .ReturnsAsync(new List<string> { "Student" });

        _courseRepositoryMock
            .Setup(repository => repository.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _enrollmentRepositoryMock
            .Setup(repository =>
                repository.GetByStudentIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enrollment?)null);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result =
            await _enrollmentService.AssignOrChangeCourseAsync(
                studentId,
                courseId
            );

        Assert.True(result);

        _enrollmentRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.Is<Enrollment>(enrollment =>
                    enrollment.StudentId == studentId &&
                    enrollment.CourseId == courseId),
                It.IsAny<CancellationToken>()),
            Times.Once
        );

        _enrollmentRepositoryMock.Verify(
            repository => repository.Update(It.IsAny<Enrollment>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task AssignOrChangeCourseAsync_WhenEnrollmentExists_UpdatesCourse()
    {
        Guid studentId = Guid.NewGuid();
        Guid oldCourseId = Guid.NewGuid();
        Guid newCourseId = Guid.NewGuid();

        var student = new User
        {
            Id = studentId,
            Name = "Test Student",
            Email = "student@example.com"
        };

        var oldCourse = new LMS.Api.Models.Course
        {
            CourseId = oldCourseId,
            Name = "Old Course"
        };

        var newCourse = new LMS.Api.Models.Course
        {
            CourseId = newCourseId,
            Name = "New Course"
        };

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = oldCourseId,
            EnrolledAt = DateTime.UtcNow.AddDays(-10),
            Course = oldCourse
        };

        _userManagerMock
            .Setup(manager => manager.FindByIdAsync(studentId.ToString()))
            .ReturnsAsync(student);

        _userManagerMock
            .Setup(manager => manager.GetRolesAsync(student))
            .ReturnsAsync(new List<string> { "Student" });

        _courseRepositoryMock
            .Setup(repository => repository.GetCourseByIdAsync(newCourseId))
            .ReturnsAsync(newCourse);

        _enrollmentRepositoryMock
            .Setup(repository =>
                repository.GetByStudentIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result =
            await _enrollmentService.AssignOrChangeCourseAsync(
                studentId,
                newCourseId
            );

        Assert.True(result);

        Assert.Equal(newCourseId, enrollment.CourseId);
        Assert.Equal(newCourse, enrollment.Course);

        _enrollmentRepositoryMock.Verify(
            repository => repository.Update(enrollment),
            Times.Once
        );

        _enrollmentRepositoryMock.Verify(
            repository => repository.AddAsync(
                It.IsAny<Enrollment>(),
                It.IsAny<CancellationToken>()),
            Times.Never
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
    [Fact]
    public async Task RemoveCourseAsync_WhenEnrollmentExists_DeletesEnrollment()
    {
        Guid studentId = Guid.NewGuid();

        var enrollment = new Enrollment
        {
            StudentId = studentId,
            CourseId = Guid.NewGuid(),
            EnrolledAt = DateTime.UtcNow
        };

        _enrollmentRepositoryMock
            .Setup(repository =>
                repository.GetByStudentIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(enrollment);

        await _enrollmentService.RemoveCourseAsync(studentId);

        _enrollmentRepositoryMock.Verify(
            repository => repository.Delete(enrollment),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveCourseAsync_WhenEnrollmentDoesNotExist_DoesNothing()
    {
        Guid studentId = Guid.NewGuid();

        _enrollmentRepositoryMock
            .Setup(repository =>
                repository.GetByStudentIdAsync(
                    studentId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Enrollment?)null);

        await _enrollmentService.RemoveCourseAsync(studentId);

        _enrollmentRepositoryMock.Verify(
            repository => repository.Delete(It.IsAny<Enrollment>()),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
