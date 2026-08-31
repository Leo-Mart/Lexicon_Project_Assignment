using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface IActivityRepository
{
    Task<List<Activity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Activity?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default);

    Task<List<Activity>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task AddAsync(Activity activity, CancellationToken cancellationToken = default);

    void Update(Activity activity);

    void Delete(Activity activity);
}