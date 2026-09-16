using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Module;

namespace LMS.Api.Services.Interfaces;

public interface IModuleService
{
    Task<PagedResponse<ModuleDto>> GetAllModules(
        QueryParametersDto query,
        CancellationToken cancellationToken = default
    );
    Task<ModuleDto?> GetModuleById(Guid moduleId);
    Task<ModuleDto> CreateNewModule(CreateNewModuleDto newModuleDto);
    Task<ModuleDto?> UpdateModule(Guid moduleId, UpdateModuleDto updateModule);
    Task<ModuleDto?> DeleteModule(Guid moduleId);

    Task<Guid?> GetCourseIdByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
}
