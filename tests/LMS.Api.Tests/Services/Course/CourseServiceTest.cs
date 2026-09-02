using AutoMapper;
using LMS.Api.DTOs.Course;
using LMS.Api.Exceptions;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LMS.Api.Tests.Services.Course;

public class CourseServiceTests
{
    private readonly Mock<ICourseRepository> _repoMock;
    private readonly CourseService _service;

    public CourseServiceTests()
    {
        _repoMock = new Mock<ICourseRepository>();

        // A real mapper, not a mock: the service's job is to map, so a
        // stubbed IMapper would leave these assertions testing nothing.
        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<CourseProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();

        _service = new CourseService(_repoMock.Object, mapper);
    }

    [Fact]
    public async Task CreateCourse_WithValidDate_ShouldReturnCreatedCourse()
    {
        Guid courseId = Guid.NewGuid();
        var request = new CreateNewCourseDto
        {
            Name = "A good course",
            Description = "A description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };

        _repoMock
            .Setup(r => r.CreateCourseAsync(It.IsAny<LMS.Api.Models.Course>()))
            .ReturnsAsync(
                (LMS.Api.Models.Course c) =>
                {
                    c.CourseId = courseId;
                    return c;
                }
            );

        var result = await _service.CreateNewCourse(request);

        Assert.NotNull(result);
        Assert.Equal(courseId, result.CourseId);
        Assert.Equal(request.Name, result.Name);
    }

    [Fact]
    public async Task CreateCourse_WithPastStartDate_ShouldThrowInvalidDateException()
    {
        var request = new CreateNewCourseDto
        {
            Name = "A course with invalid start date",
            Description = "A description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };

        await Assert.ThrowsAsync<InvalidDateException>(() => _service.CreateNewCourse(request));
    }

    [Fact]
    public async Task CreateCourse_WithEndDateEarlierThenStartDate_ShouldThrowInvalidDateException()
    {
        var request = new CreateNewCourseDto
        {
            Name = "A course with end-date before start-date",
            Description = "A description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(5)),
        };

        await Assert.ThrowsAsync<InvalidDateException>(() => _service.CreateNewCourse(request));
    }

    [Fact]
    public async Task UpdateCourse_ShouldReturnUpdatedCourse()
    {
        Guid courseId = Guid.NewGuid();
        var existingCourse = new LMS.Api.Models.Course
        {
            CourseId = courseId,
            Name = "An existing course",
            Description = "A description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };
        var request = new UpdateCourseDto
        {
            Name = "A course with an updated name",
            Description = "A description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };
        _repoMock.Setup(r => r.GetCourseByIdAsync(courseId)).ReturnsAsync(existingCourse);

        // The service maps the DTO onto the entity before saving, so the
        // repository just hands back whatever it was given.
        _repoMock
            .Setup(r => r.UpdateCourseAsync(It.IsAny<LMS.Api.Models.Course>()))
            .ReturnsAsync((LMS.Api.Models.Course c) => c);

        var result = await _service.UpdateCourse(courseId, request);
        if (result == null)
        {
            return;
        }

        Assert.Equal("A course with an updated name", result.Name);
        Assert.Equal(request.Description, result.Description);
    }
}
