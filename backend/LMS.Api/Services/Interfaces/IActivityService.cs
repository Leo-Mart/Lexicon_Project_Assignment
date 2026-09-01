using LMS.Api.DTOs.Activities;

namespace LMS.Api.Services.Interfaces;

public interface IActivityService
{
    Task<List<ActivityDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ActivityDto?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default);

    Task<List<ActivityDto>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<ActivityDto> CreateAsync(ActivityCreateDto request, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid activityId, ActivityUpdateDto request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid activityId, CancellationToken cancellationToken = default);
}
