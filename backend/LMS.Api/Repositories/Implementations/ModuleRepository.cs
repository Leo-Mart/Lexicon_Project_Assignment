using LMS.Api.Data;
using LMS.Api.DTOs.Common;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class ModuleRepository(LMSDbContext context) : IModuleRepository
{
    private readonly LMSDbContext _context = context;

    public async Task<Module> CreateModuleAsync(Module module)
    {
        module.CreatedAt = DateTime.UtcNow;
        module.UpdatedAt = DateTime.UtcNow;

        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();

        return module;
    }

    public async Task<Module?> DeleteModuleByIdAsync(Guid moduleId)
    {
        var foundModule = await _context.Modules.FirstOrDefaultAsync(m => m.ModuleId == moduleId);

        if (foundModule == null)
        {
            return null;
        }

        _context.Modules.Remove(foundModule);
        await _context.SaveChangesAsync();
        return foundModule;
    }

    public async Task<Module?> GetModuleByIdAsync(Guid moduleId)
    {
        return await _context
            .Modules.AsNoTracking()
            .AsSplitQuery()
            .Include(m => m.Course)
            .Include(m => m.Activities.OrderBy(a => a.StartAt).ThenBy(a => a.Type))
                .ThenInclude(a => a.ActivityResources)
                    .ThenInclude(ar => ar.Resource)
                        .ThenInclude(r => r.CreatedByTeacher)
            .Include(m => m.ModuleResources)
                .ThenInclude(mr => mr.Resource)
                    .ThenInclude(r => r.CreatedByTeacher)
            .FirstOrDefaultAsync((m) => m.ModuleId == moduleId);
    }

    public async Task<PagedResponse<Module>> GetModulesAsync(
        QueryParametersDto query,
        CancellationToken cancellationToken = default
    )
    {
        // return await _context
        //     .Modules.AsNoTracking()
        //     .Include(m => m.Course)
        //     .Include(m => m.Activities.OrderBy(a => a.StartAt).ThenBy(a => a.Type))
        //     .Include(m => m.ModuleResources)
        //         .ThenInclude(mr => mr.Resource)
        //             .ThenInclude(r => r.CreatedByTeacher)
        //     .OrderBy(module => module.StartDate)
        //     .ToListAsync();
        IQueryable<Module> modulesQuery = _context
            .Modules.Include(m => m.Course)
            .Include(m => m.Activities.OrderBy(a => a.StartAt).ThenBy(a => a.Type))
            .Include(m => m.ModuleResources)
                .ThenInclude(mr => mr.Resource)
                    .ThenInclude(r => r.CreatedByTeacher)
            .OrderBy(module => module.StartDate);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = query.Search.Trim();

            modulesQuery = modulesQuery.Where(module =>
                module.Name.Contains(search) || module.Description.Contains(search)
            );
        }

        modulesQuery = query.SortBy.ToLowerInvariant() switch
        {
            "createdat" => query.Direction == "desc"
                ? modulesQuery.OrderByDescending(resource => resource.CreatedAt)
                : modulesQuery.OrderBy(resource => resource.CreatedAt),

            "updatedat" => query.Direction == "desc"
                ? modulesQuery.OrderByDescending(resource => resource.UpdatedAt)
                : modulesQuery.OrderBy(resource => resource.UpdatedAt),

            _ => query.Direction == "desc"
                ? modulesQuery.OrderByDescending(resource => resource.Name)
                : modulesQuery.OrderBy(resource => resource.Name),
        };

        int totalCount = await modulesQuery.CountAsync(cancellationToken);

        List<Module> modules = await modulesQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Module>
        {
            Items = modules,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    // The module is already tracked by the service, so only the timestamp is set here.
    public async Task<Module> UpdateModuleAsync(Module module)
    {
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return module;
    }

    public async Task<Guid?> GetCourseIdByModuleIdAsync(
    Guid moduleId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Modules
            .AsNoTracking()
            .Where(module => module.ModuleId == moduleId)
            .Select(module => (Guid?)module.CourseId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
