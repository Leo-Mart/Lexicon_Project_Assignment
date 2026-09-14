using AutoMapper;
using LMS.Api.Data;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Resources;
using LMS.Api.Mappings;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LMS.Api.Tests.Services.Resources;

public class ResourceServiceTests
{
    private readonly Mock<IResourceRepository> _resourceRepositoryMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IResourceService _resourceService;
    private readonly IUserService _userService;
    private readonly LMSDbContext _context;

    public ResourceServiceTests()
    {
        _resourceRepositoryMock = new Mock<IResourceRepository>();
        _userServiceMock = new Mock<IUserService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userManagerMock = CreateUserManagerMock();

        // A real mapper, not a mock: the service's job is to map, so a
        // stubbed IMapper would leave these assertions testing nothing.
        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(ResourceProfile).Assembly),
            NullLoggerFactory.Instance
        ).CreateMapper();

        IMapper userMapper = new MapperConfiguration(
            cfg => cfg.AddMaps(typeof(UserProfile).Assembly),
            NullLoggerFactory.Instance
        ).CreateMapper();

        DbContextOptions<LMSDbContext> options = new DbContextOptionsBuilder<LMSDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new LMSDbContext(options);

        _resourceService = new ResourceService(
            _resourceRepositoryMock.Object,
            _userServiceMock.Object,
            _unitOfWorkMock.Object,
            mapper
        );

        _userService = new UserService(_userManagerMock.Object, userMapper, _context);
    }

    [Fact]
    public async Task CreateAsync_WithValidResource_ShouldCreateResource()
    {
        Guid teacherId = Guid.NewGuid();

        ResourceCreateDto dto = new()
        {
            Name = "Course documentation",
            Description = "Documentation for the course.",
            Uri = "https://example.com/documentation",
        };

        Resource? savedResource = null;
        var user = new User
        {
            Id = teacherId,
            Name = "Test User",
            Email = "test@example.com",
        };

        _userServiceMock.Setup(service => service.GetUserByIdAsync(teacherId)).ReturnsAsync(user);

        _resourceRepositoryMock
            .Setup(repository =>
                repository.AddAsync(It.IsAny<Resource>(), It.IsAny<CancellationToken>())
            )
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
            UpdatedAt = DateTime.UtcNow,
        };

        ResourceUpdateDto dto = new()
        {
            Name = resource.Name,
            Description = resource.Description,
            Uri = "https://example.com/new",
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
            Description = "Description",
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _resourceRepositoryMock
            .Setup(repository =>
                repository.AddToCourseAsync(resourceId, courseId, It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.AddToCourseAsync(resourceId, courseId);

        Assert.True(result);

        _resourceRepositoryMock.Verify(
            repository =>
                repository.AddToCourseAsync(resourceId, courseId, It.IsAny<CancellationToken>()),
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
            Description = "Description",
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _resourceRepositoryMock
            .Setup(repository =>
                repository.AddToModuleAsync(resourceId, moduleId, It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.AddToModuleAsync(resourceId, moduleId);

        Assert.True(result);

        _resourceRepositoryMock.Verify(
            repository =>
                repository.AddToModuleAsync(resourceId, moduleId, It.IsAny<CancellationToken>()),
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
            Description = "Description",
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetByIdAsync(resourceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resource);

        _resourceRepositoryMock
            .Setup(repository =>
                repository.AddToActivityAsync(resourceId, activityId, It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _resourceService.AddToActivityAsync(resourceId, activityId);

        Assert.True(result);

        _resourceRepositoryMock.Verify(
            repository =>
                repository.AddToActivityAsync(
                    resourceId,
                    activityId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPagedResources()
    {
        QueryParametersDto query = new()
        {
            Search = "",
            SortBy = "name",
            Direction = "asc",
            Page = 1,
            PageSize = 20,
        };

        List<Resource> resources =
        [
            new Resource
            {
                ResourceId = Guid.NewGuid(),
                CreatedByTeacherId = Guid.NewGuid(),
                Name = "Resource A",
                Description = "Description A",
            },
            new Resource
            {
                ResourceId = Guid.NewGuid(),
                CreatedByTeacherId = Guid.NewGuid(),
                Name = "Resource B",
                Description = "Description B",
            },
        ];

        PagedResponse<Resource> repositoryResult = new()
        {
            Items = resources,
            TotalCount = 2,
            Page = 1,
            PageSize = 20,
        };

        _resourceRepositoryMock
            .Setup(repository => repository.GetAllAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        PagedResponse<ResourceDto> result = await _resourceService.GetAllAsync(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(20, result.PageSize);

        Assert.Equal("Resource A", result.Items[0].Name);
        Assert.Equal("Resource B", result.Items[1].Name);
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
}
