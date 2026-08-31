using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface IResourceRepository
{
    Task<List<Resource>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Resource?> GetByIdAsync(Guid resourceId, CancellationToken cancellationToken = default);

    Task<List<Resource>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<List<Resource>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<List<Resource>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);

    Task AddAsync(Resource resource, CancellationToken cancellationToken = default);

    void Update(Resource resource);

    void Delete(Resource resource);

    Task AddToCourseAsync(Guid resourceId, Guid courseId, CancellationToken cancellationToken = default);

    Task AddToModuleAsync(Guid resourceId, Guid moduleId, CancellationToken cancellationToken = default);

    Task AddToActivityAsync(Guid resourceId, Guid activityId, CancellationToken cancellationToken = default);
}
