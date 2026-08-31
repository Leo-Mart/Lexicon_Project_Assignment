using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Resources;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Moq;

namespace LMS.Api.Tests.Services.Resources;

public class ResourceServiceTests
{
    private readonly Mock<IResourceRepository> _resourceRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IResourceService _resourceService;

    public ResourceServiceTests()
    {
        _resourceRepositoryMock = new Mock<IResourceRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _resourceService = new ResourceService(
            _resourceRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateAsync_WithValidResource_ShouldCreateResource()
    {
        Guid teacherId = Guid.NewGuid();

        ResourceCreateDto dto = new()
        {
            Name = "Course documentation",
            Description = "Documentation for the course.",
            Uri = "https://example.com/documentation"
        };

        Resource? savedResource = null;

        _resourceRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Resource>(), It.IsAny<CancellationToken>()))
            .Callback<Resource, CancellationToken>((resource, _) => savedResource = resource)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        ResourceDto result = await _resourceService.CreateAsync(teacherId, dto);

        Assert.NotNull(savedResource);
        Assert.Equal(dto.Name, savedResource.Name);
        Assert.Equal(dto.Description, savedResource.Description);
        Assert.Equal(dto.Uri, savedResource.Uri);
        Assert.Equal(teacherId, savedResource.CreatedByTeacherId);

        Assert.Equal(dto.Name, result.Name);
        Assert.Equal(teacherId, result.CreatedByTeacherId);

        _resourceRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<Resource>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WithNewUri_ShouldUpdateUri()
    {
        Guid resourceId = Guid.NewGuid();

        Resource resource = new()
        {
            ResourceId = resourceId,
            CreatedByTeacherId = Guid.NewGuid(),
            Name = "Course documentation",
            Description = "Documentation",
            Uri = "https://example.com/old",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ResourceUpdateDto dto = new()
        {
            Name = resource.Name,
            Description = resource.Description,
            Uri = "https://example.com/new"
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.UpdateAsync(resourceId, dto);

        Assert.True(result);
        Assert.Equal("https://example.com/new", resource.Uri);

        _resourceRepositoryMock.Verify(repository => repository.Update(resource), Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task AddToCourseAsync_WithExistingResource_ShouldAddRelation()
    {
        Guid resourceId = Guid.NewGuid();
        Guid courseId = Guid.NewGuid();

        Resource resource = new()
        {
            ResourceId = resourceId,
            CreatedByTeacherId = Guid.NewGuid(),
            Name = "Course resource",
            Description = "Description"
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _resourceRepositoryMock
            .Setup(repository => repository.AddToCourseAsync(resourceId, courseId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.AddToCourseAsync(resourceId, courseId);

        Assert.True(result);

        _resourceRepositoryMock.Verify(
            repository => repository.AddToCourseAsync(resourceId, courseId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task AddToModuleAsync_WithExistingResource_ShouldAddRelation()
    {
        Guid resourceId = Guid.NewGuid();
        Guid moduleId = Guid.NewGuid();

        Resource resource = new()
        {
            ResourceId = resourceId,
            CreatedByTeacherId = Guid.NewGuid(),
            Name = "Module resource",
            Description = "Description"
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _resourceRepositoryMock
            .Setup(repository => repository.AddToModuleAsync(resourceId, moduleId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.AddToModuleAsync(resourceId, moduleId);

        Assert.True(result);

        _resourceRepositoryMock.Verify(
            repository => repository.AddToModuleAsync(resourceId, moduleId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task AddToActivityAsync_WithExistingResource_ShouldAddRelation()
    {
        Guid resourceId = Guid.NewGuid();
        Guid activityId = Guid.NewGuid();

        Resource resource = new()
        {
            ResourceId = resourceId,
            CreatedByTeacherId = Guid.NewGuid(),
            Name = "Activity resource",
            Description = "Description"
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _resourceRepositoryMock
            .Setup(repository => repository.AddToActivityAsync(resourceId, activityId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.AddToActivityAsync(resourceId, activityId);

        Assert.True(result);

        _resourceRepositoryMock.Verify(
            repository => repository.AddToActivityAsync(resourceId, activityId, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
