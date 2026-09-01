using LMS.Api.Data;
using LMS.Api.Repositories.Interfaces.Module;
using Microsoft.EntityFrameworkCore;
using ModuleEntity = LMS.Api.Models.Module;

namespace LMS.Api.Repositories.Implementations.Module
{
    public class ModuleRepository(LMSDbContext context) : IModuleRepository
    {
        private readonly LMSDbContext _context = context;

        public async Task<ModuleEntity> CreateModuleAsync(ModuleEntity module)
        {
            module.CreatedAt = DateTime.UtcNow;
            module.UpdatedAt = DateTime.UtcNow;

            await _context.Modules.AddAsync(module);
            await _context.SaveChangesAsync();

            return module;
        }

        public async Task<ModuleEntity?> DeleteModuleByIdAsync(Guid moduleId)
        {
            var foundModule = await _context.Modules.FirstOrDefaultAsync(m =>
                m.ModuleId == moduleId
            );

            if (foundModule == null)
            {
                return null;
            }

            _context.Modules.Remove(foundModule);
            await _context.SaveChangesAsync();
            return foundModule;
        }

        public async Task<ModuleEntity?> GetModuleByIdAsync(Guid moduleId)
        {
            return await _context.Modules.FirstOrDefaultAsync((m) => m.ModuleId == moduleId);
        }

        public async Task<IEnumerable<ModuleEntity>> GetModulesAsync()
        {
            return await _context.Modules.ToListAsync();
        }

        // The module is already tracked by the service, so only the timestamp is set here.
        public async Task<ModuleEntity> UpdateModuleAsync(ModuleEntity module)
        {
            module.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return module;
        }
    }
}
