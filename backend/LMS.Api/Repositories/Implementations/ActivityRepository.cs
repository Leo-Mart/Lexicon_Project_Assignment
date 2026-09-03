using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class ActivityRepository : IActivityRepository
{
    private readonly LMSDbContext _context;

    public ActivityRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<List<Activity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Activity?> GetByIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .AsNoTracking()
            .OrderBy(activity => activity.StartAt)
            .FirstOrDefaultAsync(activity => activity.ActivityId == activityId, cancellationToken);
    }

    public async Task<List<Activity>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Activities
            .AsNoTracking()
            .Where(activity => activity.ModuleId == moduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Activity activity, CancellationToken cancellationToken = default)
    {
        await _context.Activities.AddAsync(activity, cancellationToken);
    }

    public void Update(Activity activity)
    {
        _context.Activities.Update(activity);
    }

    public void Delete(Activity activity)
    {
        _context.Activities.Remove(activity);
    }
}
