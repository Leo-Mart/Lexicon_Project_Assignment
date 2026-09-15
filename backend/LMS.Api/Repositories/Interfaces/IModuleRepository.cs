using LMS.Api.DTOs.Common;
using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface IModuleRepository
{
    Task<PagedResponse<Module>> GetModulesAsync(
        QueryParametersDto query,
        CancellationToken cancellationToken = default
    );
    Task<Module?> GetModuleByIdAsync(Guid moduleId);
    Task<Module?> GetModuleForUpdateAsync(Guid moduleId);
    Task<Module> CreateModuleAsync(Module module);
    Task<Module> UpdateModuleAsync(Module module);
    Task<Module?> DeleteModuleByIdAsync(Guid moduleId);

    Task<Guid?> GetCourseIdByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

}
