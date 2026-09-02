using System.ComponentModel.DataAnnotations;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Activities;
using LMS.Api.Exceptions;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;
using LMS.Api.Validators;

namespace LMS.Api.Services.Implementations;

public class ActivityService : IActivityService
{
    private readonly IActivityRepository _activityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IModuleRepository _moduleRepository;

    public ActivityService(
        IActivityRepository activityRepository,
        IUnitOfWork unitOfWork,
        IModuleRepository moduleRepository
    )
    {
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
        _moduleRepository = moduleRepository;
    }

    public async Task<List<ActivityDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Activity> activities = await _activityRepository.GetAllAsync(cancellationToken);

        return activities.Select(MapToDto).ToList();
    }

    public async Task<ActivityDto?> GetByIdAsync(
        Guid activityId,
        CancellationToken cancellationToken = default
    )
    {
        Activity? activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        return activity is null ? null : MapToDto(activity);
    }

    public async Task<List<ActivityDto>> GetByModuleIdAsync(
        Guid moduleId,
        CancellationToken cancellationToken = default
    )
    {
        List<Activity> activities = await _activityRepository.GetByModuleIdAsync(
            moduleId,
            cancellationToken
        );

        return activities.Select(MapToDto).ToList();
    }

    public async Task<ActivityDto> CreateAsync(
        ActivityCreateDto request,
        CancellationToken cancellationToken = default
    )
    {
        await ValidateActivityDatesAsync(
            request.ModuleId,
            request.StartAt,
            request.EndAt,
            validateNotBefore: true,
            cancellationToken: cancellationToken
        );

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
            UpdatedAt = now,
        };

        await _activityRepository.AddAsync(activity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(activity);
    }

    public async Task<bool> UpdateAsync(
        Guid activityId,
        ActivityUpdateDto request,
        CancellationToken cancellationToken = default
    )
    {
        Activity? activity = await _activityRepository.GetByIdAsync(activityId, cancellationToken);

        if (activity is null)
        {
            return false;
        }

        await ValidateActivityDatesAsync(
            activity.ModuleId,
            request.StartAt,
            request.EndAt,
            excludedActivityId: activityId,
            cancellationToken: cancellationToken
        );

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

    public async Task<bool> DeleteAsync(
        Guid activityId,
        CancellationToken cancellationToken = default
    )
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
            Deadline = activity.Deadline,
        };
    }

    private async Task ValidateActivityDatesAsync(
        Guid moduleId,
        DateTime startAt,
        DateTime endAt,
        Guid? excludedActivityId = null,
        bool validateNotBefore = false,
        CancellationToken cancellationToken = default
    )
    {
        DateRangeValidator.ValidateRange(startAt, endAt, "Activity");

        if (validateNotBefore)
        {
            DateRangeValidator.ValidateNotBefore(startAt, DateTime.UtcNow, "Activity");
        }

        Module? module = await _moduleRepository.GetModuleByIdAsync(moduleId);

        if (module is null)
        {
            throw new KeyNotFoundException("Module not found.");
        }

        DateTime moduleStart = module.StartDate.ToDateTime(TimeOnly.MinValue);
        DateTime moduleEnd = module.EndDate.ToDateTime(TimeOnly.MaxValue);

        DateRangeValidator.ValidateWithinParent(
            startAt,
            endAt,
            moduleStart,
            moduleEnd,
            "Activity",
            "Module"
        );

        List<Activity> existingActivities = await _activityRepository.GetByModuleIdAsync(
            moduleId,
            cancellationToken
        );

        foreach (Activity existingActivity in existingActivities)
        {
            if (
                excludedActivityId.HasValue
                && existingActivity.ActivityId == excludedActivityId.Value
            )
            {
                continue;
            }

            bool overlaps = DateRangeValidator.Overlaps(
                startAt,
                endAt,
                existingActivity.StartAt,
                existingActivity.EndAt
            );

            if (overlaps)
            {
                throw new InvalidDateException(
                    $"Activity overlaps with existing activity: {existingActivity.Name}.",
                    400
                );
            }
        }
    }
}
