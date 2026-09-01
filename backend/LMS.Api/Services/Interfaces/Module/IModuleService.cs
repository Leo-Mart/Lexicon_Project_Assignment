using LMS.Api.DTOs.Module;

namespace LMS.Api.Services.Interfaces.Module
{
    public interface IModuleService
    {
        Task<IEnumerable<ModuleDto>?> GetAllModules();
        Task<ModuleDto?> GetModuleById(Guid moduleId);
        Task<ModuleDto> CreateNewModule(CreateNewModuleDto newModuleDto);
        Task<ModuleDto?> UpdateModule(Guid moduleId, UpdateModuleDto updateModule);
        Task<ModuleDto?> DeleteModule(Guid moduleId);
    }
}
