using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Activities;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Moq;

namespace LMS.Api.Tests.Services;

public class ActivityServiceTests
{
    private readonly Mock<IActivityRepository> _activityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IActivityService _activityService;

    public ActivityServiceTests()
    {
        _activityRepositoryMock = new Mock<IActivityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _activityService = new ActivityService(
            _activityRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnActivities()
    {
        List<Activity> activities =
        [
            CreateActivity("Activity 1"),
            CreateActivity("Activity 2")
        ];

        _activityRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activities);

        List<ActivityDto> result = await _activityService.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Activity 1", result[0].Name);
        Assert.Equal("Activity 2", result[1].Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingActivity_ShouldReturnActivity()
    {
        Activity activity = CreateActivity("Activity");

        _activityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activity.ActivityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        ActivityDto? result = await _activityService.GetByIdAsync(activity.ActivityId);

        Assert.NotNull(result);
        Assert.Equal(activity.ActivityId, result.ActivityId);
        Assert.Equal(activity.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingActivity_ShouldReturnNull()
    {
        Guid activityId = Guid.NewGuid();

        _activityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Activity?)null);

        ActivityDto? result = await _activityService.GetByIdAsync(activityId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByModuleIdAsync_ShouldReturnActivitiesForModule()
    {
        Guid moduleId = Guid.NewGuid();

        List<Activity> activities =
        [
            CreateActivity("Activity 1", moduleId),
            CreateActivity("Activity 2", moduleId)
        ];

        _activityRepositoryMock
            .Setup(repository => repository.GetByModuleIdAsync(moduleId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activities);

        List<ActivityDto> result = await _activityService.GetByModuleIdAsync(moduleId);

        Assert.Equal(2, result.Count);
        Assert.All(result, activity => Assert.Equal(moduleId, activity.ModuleId));
    }

    [Fact]
    public async Task CreateAsync_WithValidActivity_ShouldCreateActivity()
    {
        Guid moduleId = Guid.NewGuid();
        DateTime startAt = DateTime.UtcNow;
        DateTime endAt = startAt.AddHours(2);

        ActivityCreateDto dto = new()
        {
            ModuleId = moduleId,
            Type = ActivityType.Lecture,
            Name = "New activity",
            Description = "Activity description",
            StartAt = startAt,
            EndAt = endAt
        };

        Activity? savedActivity = null;

        _activityRepositoryMock
            .Setup(repository => repository.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()))
            .Callback<Activity, CancellationToken>((activity, _) => savedActivity = activity)
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        ActivityDto result = await _activityService.CreateAsync(dto);

        Assert.NotNull(savedActivity);
        Assert.NotEqual(Guid.Empty, savedActivity.ActivityId);
        Assert.Equal(moduleId, savedActivity.ModuleId);
        Assert.Equal(dto.Type, savedActivity.Type);
        Assert.Equal(dto.Name, savedActivity.Name);
        Assert.Equal(dto.Description, savedActivity.Description);
        Assert.Equal(startAt, savedActivity.StartAt);
        Assert.Equal(endAt, savedActivity.EndAt);
        Assert.NotEqual(default, savedActivity.CreatedAt);
        Assert.Equal(savedActivity.CreatedAt, savedActivity.UpdatedAt);

        Assert.Equal(savedActivity.ActivityId, result.ActivityId);

        _activityRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<Activity>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WithExistingActivity_ShouldUpdateActivity()
    {
        Activity activity = CreateActivity("Old name");

        DateTime newStartAt = DateTime.UtcNow.AddDays(1);
        DateTime newEndAt = newStartAt.AddHours(3);

        ActivityUpdateDto dto = new()
        {
            Type = ActivityType.Lecture,
            Name = "Updated name",
            Description = "Updated description",
            StartAt = newStartAt,
            EndAt = newEndAt,
            Deadline = newStartAt.AddDays(1)
        };

        _activityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activity.ActivityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _activityService.UpdateAsync(activity.ActivityId, dto);

        Assert.True(result);
        Assert.Equal(dto.Type, activity.Type);
        Assert.Equal(dto.Name, activity.Name);
        Assert.Equal(dto.Description, activity.Description);
        Assert.Equal(dto.StartAt, activity.StartAt);
        Assert.Equal(dto.EndAt, activity.EndAt);
        Assert.Equal(dto.Deadline, activity.Deadline);

        _activityRepositoryMock.Verify(repository => repository.Update(activity), Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingActivity_ShouldReturnFalse()
    {
        Guid activityId = Guid.NewGuid();

        ActivityUpdateDto dto = new()
        {
            Type = ActivityType.Lecture,
            Name = "Activity",
            Description = "Description",
            StartAt = DateTime.UtcNow,
            EndAt = DateTime.UtcNow.AddHours(2)
        };

        _activityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Activity?)null);

        bool result = await _activityService.UpdateAsync(activityId, dto);

        Assert.False(result);

        _activityRepositoryMock.Verify(repository => repository.Update(It.IsAny<Activity>()), Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteAsync_WithExistingActivity_ShouldDeleteActivity()
    {
        Activity activity = CreateActivity("Activity");

        _activityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activity.ActivityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activity);

        _unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        bool result = await _activityService.DeleteAsync(activity.ActivityId);

        Assert.True(result);

        _activityRepositoryMock.Verify(repository => repository.Delete(activity), Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingActivity_ShouldReturnFalse()
    {
        Guid activityId = Guid.NewGuid();

        _activityRepositoryMock
            .Setup(repository => repository.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Activity?)null);

        bool result = await _activityService.DeleteAsync(activityId);

        Assert.False(result);

        _activityRepositoryMock.Verify(repository => repository.Delete(It.IsAny<Activity>()), Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    private static Activity CreateActivity(string name, Guid? moduleId = null)
    {
        DateTime now = DateTime.UtcNow;

        return new Activity
        {
            ActivityId = Guid.NewGuid(),
            ModuleId = moduleId ?? Guid.NewGuid(),
            Type = ActivityType.Lecture,
            Name = name,
            Description = "Description",
            StartAt = now,
            EndAt = now.AddHours(2),
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
