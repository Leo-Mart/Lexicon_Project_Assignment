using ModuleEntity = LMS.Api.Models.Module;

namespace LMS.Api.Repositories.Interfaces.Module
{
    public interface IModuleRepository
    {
        Task<IEnumerable<ModuleEntity>> GetModulesAsync();
        Task<ModuleEntity?> GetModuleByIdAsync(Guid moduleId);
        Task<ModuleEntity> CreateModuleAsync(ModuleEntity module);
        Task<ModuleEntity> UpdateModuleAsync(ModuleEntity module);
        Task<ModuleEntity?> DeleteModuleByIdAsync(Guid moduleId);
    }
}
