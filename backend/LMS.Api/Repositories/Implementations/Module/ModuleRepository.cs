using LMS.Api.Data;
using LMS.Api.DTOs.Module;
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

        public async Task<ModuleEntity?> UpdateModuleAsync(
            Guid moduleId,
            UpdateModuleDto updateModuleDto
        )
        {
            var moduleFromDb = await _context.Modules.FirstOrDefaultAsync(m =>
                m.ModuleId == moduleId
            );
            if (moduleFromDb == null)
            {
                return null;
            }

            moduleFromDb.Name = updateModuleDto.Name;
            moduleFromDb.Description = updateModuleDto.Description;
            moduleFromDb.StartDate = updateModuleDto.StartDate;
            moduleFromDb.EndDate = updateModuleDto.EndDate;
            moduleFromDb.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return moduleFromDb;
        }
    }
}
