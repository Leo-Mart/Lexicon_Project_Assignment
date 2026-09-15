using AutoMapper;
using LMS.Api.DTOs.Module;
using LMS.Api.Exceptions;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using CourseEntity = LMS.Api.Models.Course;
using ModuleEntity = LMS.Api.Models.Module;

namespace LMS.Api.Tests.Services.Module;

public class ModuleServiceTests
{
    private readonly Mock<IModuleRepository> _mockModuleRepo;
    private readonly Mock<ICourseRepository> _mockCourseRepo;
    private readonly ModuleService _service;

    public ModuleServiceTests()
    {
        _mockModuleRepo = new Mock<IModuleRepository>();
        _mockCourseRepo = new Mock<ICourseRepository>();

        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<ModuleProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();

        _service = new ModuleService(_mockModuleRepo.Object, _mockCourseRepo.Object, mapper);
    }

    [Fact]
    public async Task CreateModule_WithValidDateWithinCourseTimeframe_ShouldReturnCreatedModule()
    {
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var request = new CreateNewModuleDto
        {
            CourseId = courseId,
            Name = "A good module",
            Description = "A Description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };

        _mockCourseRepo
            .Setup(r => r.GetCourseByIdAsync(courseId))
            .ReturnsAsync(
                new CourseEntity
                {
                    CourseId = courseId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(21)),
                }
            );

        _mockModuleRepo
            .Setup(r => r.CreateModuleAsync(It.IsAny<ModuleEntity>()))
            .ReturnsAsync(
                (ModuleEntity m) =>
                {
                    m.ModuleId = moduleId;
                    return m;
                }
            );

        var result = await _service.CreateNewModule(request);

        Assert.NotNull(result);
        Assert.Equal(moduleId, result.ModuleId);
        Assert.Equal(request.Name, result.Name);
    }

    [Fact]
    public async Task CreateModule_WithDateOutsideCourseTimeframe_ShouldThrowError()
    {
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var request = new CreateNewModuleDto
        {
            CourseId = courseId,
            Name = "A bad module",
            Description = "a module that starts before the course does",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-8)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };

        _mockCourseRepo
            .Setup(r => r.GetCourseByIdAsync(courseId))
            .ReturnsAsync(
                new CourseEntity
                {
                    CourseId = courseId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(21)),
                }
            );

        _mockModuleRepo
            .Setup(r => r.CreateModuleAsync(It.IsAny<ModuleEntity>()))
            .ReturnsAsync(
                (ModuleEntity m) =>
                {
                    m.ModuleId = moduleId;
                    return m;
                }
            );

        await Assert.ThrowsAsync<InvalidDateException>(() => _service.CreateNewModule(request));
    }

    [Fact]
    public async Task CreateModule_WithExistingModuleWithOverlappingDates_ShouldThrowError()
    {
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var request = new CreateNewModuleDto
        {
            CourseId = courseId,
            Name = "A bad module",
            Description = "A module whose dates overlap with those of an existing module",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };

        _mockCourseRepo
            .Setup(r => r.GetCourseByIdAsync(courseId))
            .ReturnsAsync(
                new CourseEntity
                {
                    CourseId = courseId,
                    StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-7)),
                    EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(21)),
                    Modules = new List<ModuleEntity>
                    {
                        new ModuleEntity
                        {
                            CourseId = courseId,
                            Name = "An existing module",
                            Description = "A module that already exists on the course",
                            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(1)),
                            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(20)),
                        },
                    },
                }
            );

        _mockModuleRepo
            .Setup(r => r.CreateModuleAsync(It.IsAny<ModuleEntity>()))
            .ReturnsAsync(
                (ModuleEntity m) =>
                {
                    m.ModuleId = moduleId;
                    return m;
                }
            );

        await Assert.ThrowsAsync<OverlappingDateException>(() => _service.CreateNewModule(request));
    }

    [Fact]
    public async Task GetCourseIdByModuleIdAsync_WhenModuleExists_ReturnsCourseId()
    {
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        _mockModuleRepo
            .Setup(repository =>
                repository.GetCourseIdByModuleIdAsync(
                    moduleId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(courseId);

        Guid? result = await _service.GetCourseIdByModuleIdAsync(moduleId);

        Assert.Equal(courseId, result);

        _mockModuleRepo.Verify(
            repository =>
                repository.GetCourseIdByModuleIdAsync(
                    moduleId,
                    It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetCourseIdByModuleIdAsync_WhenModuleDoesNotExist_ReturnsNull()
    {
        Guid moduleId = Guid.NewGuid();

        _mockModuleRepo
            .Setup(repository =>
                repository.GetCourseIdByModuleIdAsync(
                    moduleId,
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        Guid? result = await _service.GetCourseIdByModuleIdAsync(moduleId);

        Assert.Null(result);
    }
    [Fact]
    public async Task UpdateModule_WithValidData_ShouldReturnUpdatedModule()
    {
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var existingModule = new ModuleEntity
        {
            ModuleId = moduleId,
            CourseId = courseId,
            Name = "Old Module",
            Description = "Old description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
        };

        var course = new CourseEntity
        {
            CourseId = courseId,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
            Modules = new List<ModuleEntity>
            {
                existingModule
            }
        };

        var request = new UpdateModuleDto
        {
            CourseId = courseId,
            Name = "Updated Module",
            Description = "Updated description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(8)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(15)),
        };

        _mockModuleRepo
            .Setup(repository => repository.GetModuleForUpdateAsync(moduleId))
            .ReturnsAsync(existingModule);

        _mockCourseRepo
            .Setup(repository => repository.GetCourseByIdAsync(courseId))
            .ReturnsAsync(course);

        _mockModuleRepo
            .Setup(repository =>
                repository.UpdateModuleAsync(It.IsAny<ModuleEntity>()))
            .ReturnsAsync((ModuleEntity module) => module);

        ModuleDto? result =
            await _service.UpdateModule(moduleId, request);

        Assert.NotNull(result);
        Assert.Equal("Updated Module", result.Name);
        Assert.Equal("Updated description", result.Description);

        _mockModuleRepo.Verify(
            repository => repository.GetModuleForUpdateAsync(moduleId),
            Times.Once
        );

        _mockModuleRepo.Verify(
            repository => repository.UpdateModuleAsync(existingModule),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateModule_WhenModuleDoesNotExist_ReturnsNull()
    {
        Guid moduleId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        var request = new UpdateModuleDto
        {
            Name = "Updated Module",
            Description = "Updated description",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
            CourseId = courseId,
        };

        _mockModuleRepo
            .Setup(repository => repository.GetModuleForUpdateAsync(moduleId))
            .ReturnsAsync((ModuleEntity?)null);

        ModuleDto? result =
            await _service.UpdateModule(moduleId, request);

        Assert.Null(result);

        _mockModuleRepo.Verify(
            repository => repository.GetModuleForUpdateAsync(moduleId),
            Times.Once
        );

        _mockModuleRepo.Verify(
            repository => repository.UpdateModuleAsync(It.IsAny<ModuleEntity>()),
            Times.Never
        );
    }
}
