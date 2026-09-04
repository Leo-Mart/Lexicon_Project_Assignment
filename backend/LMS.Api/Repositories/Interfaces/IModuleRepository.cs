using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface IModuleRepository
{
    Task<IEnumerable<Module>> GetModulesAsync();
    Task<Module?> GetModuleByIdAsync(Guid moduleId);
    Task<Module> CreateModuleAsync(Module module);
    Task<Module> UpdateModuleAsync(Module module);
    Task<Module?> DeleteModuleByIdAsync(Guid moduleId);
}
