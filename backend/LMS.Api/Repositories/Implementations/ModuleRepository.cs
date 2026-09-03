using LMS.Api.Data;
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
            .Modules.Include(m => m.Course)
            .Include(m => m.Activities)
            .FirstOrDefaultAsync((m) => m.ModuleId == moduleId);
    }

    public async Task<IEnumerable<Module>> GetModulesAsync()
    {
        return await _context
            .Modules.Include(m => m.Course)
            .Include(m => m.Activities)
            .OrderBy(module => module.StartDate)
            .ToListAsync();
    }

    // The module is already tracked by the service, so only the timestamp is set here.
    public async Task<Module> UpdateModuleAsync(Module module)
    {
        module.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return module;
    }
}
