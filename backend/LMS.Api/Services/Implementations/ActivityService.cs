using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Activities;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivityService(IActivityRepository activityRepository, IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ActivityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Activity> activities = await _activityRepository.GetAllAsync(cancellationToken);

        return activities.Select(MapToDto).ToList();
    }

    public async Task<ActivityDto?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        Activity? activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        return activity is null ? null : MapToDto(activity);
    }

    public async Task<List<ActivityDto>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        List<Activity> activities = await _activityRepository.GetByModuleIdAsync(moduleId, cancellationToken);

        return activities.Select(MapToDto).ToList();
    }

    public async Task<ActivityDto> CreateAsync(ActivityCreateDto request, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;

        Activity activity = new()
        {
            ActivityId = Guid.NewGuid(),
            ModuleId = request.ModuleId,
            Type = request.Type,
            Name = request.Name,
            Description = request.Description,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Deadline = request.Deadline,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _activityRepository.AddAsync(activity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(activity);
    }

    public async Task<bool> UpdateAsync(Guid activityId, ActivityUpdateDto request, CancellationToken cancellationToken = default)
    {
        Activity? activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        if (activity is null)
        {
            return false;
        }

        activity.Type = request.Type;
        activity.Name = request.Name;
        activity.Description = request.Description;
        activity.StartAt = request.StartAt;
        activity.EndAt = request.EndAt;
        activity.Deadline = request.Deadline;
        activity.UpdatedAt = DateTime.UtcNow;

        _activityRepository.Update(activity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        Activity? activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        if (activity is null)
        {
            return false;
        }

        _activityRepository.Delete(activity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ActivityDto MapToDto(Activity activity)
    {
        return new ActivityDto
        {
            ActivityId = activity.ActivityId,
            ModuleId = activity.ModuleId,
            Type = activity.Type,
            Name = activity.Name,
            Description = activity.Description,
            StartAt = activity.StartAt,
            EndAt = activity.EndAt,
            CreatedAt = activity.CreatedAt,
            UpdatedAt = activity.UpdatedAt,
            Deadline = activity.Deadline
        };
    }
}
