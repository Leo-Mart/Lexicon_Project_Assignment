using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class ResourceRepository : IResourceRepository
{
    private readonly LMSDbContext _context;

    public ResourceRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<List<Resource>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Resource?> GetByIdAsync(Guid resourceId, CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .AsNoTracking()
            .FirstOrDefaultAsync(
                resource => resource.ResourceId == resourceId,
                cancellationToken
            );
    }

    public async Task<List<Resource>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .AsNoTracking()
            .Where(resource =>
                resource.CourseResources.Any(
                    courseResource => courseResource.CourseId == courseId
                )
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Resource>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .AsNoTracking()
            .Where(resource =>
                resource.ModuleResources.Any(
                    moduleResource => moduleResource.ModuleId == moduleId
                )
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Resource>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return await _context.Resources
            .AsNoTracking()
            .Where(resource =>
                resource.ActivityResources.Any(
                    activityResource => activityResource.ActivityId == activityId
                )
            )
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Resource resource, CancellationToken cancellationToken = default)
    {
        await _context.Resources.AddAsync(resource, cancellationToken);
    }

    public void Update(Resource resource)
    {
        _context.Resources.Update(resource);
    }

    public void Delete(Resource resource)
    {
        _context.Resources.Remove(resource);
    }
}
