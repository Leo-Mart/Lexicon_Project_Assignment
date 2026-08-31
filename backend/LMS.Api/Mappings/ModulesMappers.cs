using LMS.Api.DTOs.Module;
using LMS.Api.Models;

namespace LMS.Api.Mappings
{
    public static class ModuleMappers
    {
        public static ModuleDto ToDtoFromModule(this Module module)
        {
            return new ModuleDto
            {
                ModuleId = module.ModuleId,
                CourseId = module.CourseId,
                Name = module.Name,
                Description = module.Description,
                StartDate = module.StartDate,
                EndDate = module.EndDate,
            };
        }

        public static Module ToModuleFromCreate(this CreateNewModuleDto newModuleDto)
        {
            return new Module
            {
                CourseId = newModuleDto.CourseId,
                Name = newModuleDto.Name,
                Description = newModuleDto.Description,
                StartDate = newModuleDto.StartDate,
                EndDate = newModuleDto.EndDate,
            };
        }
    }
}
