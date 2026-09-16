using LMS.Api.Data;
using LMS.Api.DTOs.Common;
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

    public async Task<PagedResponse<Activity>> GetAllAsync(
        QueryParametersDto query,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Activity> activitiesQuery = _context.Activities.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = query.Search.Trim();

            activitiesQuery = activitiesQuery.Where(activity =>
                activity.Name.Contains(search) || activity.Description.Contains(search)
            );
        }

        activitiesQuery = query.SortBy.ToLowerInvariant() switch
        {
            "createdat" => query.Direction == "desc"
                ? activitiesQuery.OrderByDescending(activity => activity.CreatedAt)
                : activitiesQuery.OrderBy(activity => activity.CreatedAt),

            "updatedat" => query.Direction == "desc"
                ? activitiesQuery.OrderByDescending(activity => activity.UpdatedAt)
                : activitiesQuery.OrderBy(activity => activity.UpdatedAt),

            _ => query.Direction == "desc"
                ? activitiesQuery.OrderByDescending(activity => activity.Name)
                : activitiesQuery.OrderBy(activity => activity.Name),
        };

        int totalCount = await activitiesQuery.CountAsync(cancellationToken);

        List<Activity> activities = await activitiesQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Activity>
        {
            Items = activities,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<Activity?> GetByIdAsync(
        Guid activityId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Activities.AsNoTracking()
            .OrderBy(activity => activity.StartAt)
            .FirstOrDefaultAsync(activity => activity.ActivityId == activityId, cancellationToken);
    }

    public async Task<List<Activity>> GetByModuleIdAsync(
        Guid moduleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Activities.AsNoTracking()
            .Include(a => a.ActivityResources)
                .ThenInclude(ar => ar.Resource)
                    .ThenInclude(r => r.CreatedByTeacher)
            .Where(activity => activity.ModuleId == moduleId)
            .OrderBy(activity => activity.StartAt)
            .ThenBy(activity => activity.Type)
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
