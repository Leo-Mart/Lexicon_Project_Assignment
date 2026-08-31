using LMS.Api.DTOs.Resources;

namespace LMS.Api.Services.Interfaces;

public interface IResourceService
{
    Task<List<ResourceDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<ResourceDto?> GetByIdAsync(Guid resourceId, CancellationToken cancellationToken = default);

    Task<List<ResourceDto>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<List<ResourceDto>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<List<ResourceDto>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);

    Task<ResourceDto> CreateAsync(
        Guid createdByTeacherId,
        ResourceCreateDto request,
        CancellationToken cancellationToken = default
    );

    Task<bool> UpdateAsync(
        Guid resourceId,
        ResourceUpdateDto request,
        CancellationToken cancellationToken = default
    );

    Task<bool> DeleteAsync(Guid resourceId, CancellationToken cancellationToken = default);

    Task<bool> AddToCourseAsync(Guid resourceId, Guid courseId, CancellationToken cancellationToken = default);

    Task<bool> AddToModuleAsync(Guid resourceId, Guid moduleId, CancellationToken cancellationToken = default);

    Task<bool> AddToActivityAsync(Guid resourceId, Guid activityId, CancellationToken cancellationToken = default);
}
