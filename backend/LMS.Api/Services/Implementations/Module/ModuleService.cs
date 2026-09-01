using AutoMapper;
using LMS.Api.DTOs.Module;
using LMS.Api.Exceptions;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces.Course;
using LMS.Api.Repositories.Interfaces.Module;
using LMS.Api.Services.Interfaces.Module;
using ModuleEntity = LMS.Api.Models.Module;

namespace LMS.Api.Services.Implementations
{
    public class ModuleService(
        IModuleRepository moduleRepo,
        ICourseRepository courseRepo,
        IMapper mapper
    ) : IModuleService
    {
        private readonly IModuleRepository _moduleRepo = moduleRepo;
        private readonly ICourseRepository _courseRepo = courseRepo;
        private readonly IMapper _mapper = mapper;

        public async Task<ModuleDto> CreateNewModule(CreateNewModuleDto newModule)
        {
            if (newModule.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                throw new InvalidDateException("Start date cannot be in the past.", 400);
            }
            int result = newModule.EndDate.CompareTo(newModule.StartDate);
            if (result < 0 || result == 0)
            {
                throw new InvalidDateException("End date has to be in the future.", 400);
            }

            var course = await _courseRepo.GetCourseByIdAsync(newModule.CourseId);
            if (course == null)
            {
                throw new ArgumentException("Could not find course.");
            }

            //TODO: Break out into date-check helpers?
            // bool isWithinCourseTimeframe =
            //     course.StartDate < newModule.StartDate && course.EndDate > newModule.EndDate;
            //
            // if (isWithinCourseTimeframe)
            // {
            //     throw new ArgumentException(
            //         $"Could not create module. Module start or end-date sits outside the timeframe of the course."
            //     );
            // }
            //

            foreach (var module in course.Modules)
            {
                bool overlaps =
                    module.StartDate < newModule.EndDate && newModule.StartDate < module.EndDate;

                if (overlaps)
                {
                    throw new OverlappingDateException(
                        $"Could not create module. Dates overlap with existing module: {module.Name}",
                        400
                    );
                }
            }

            var moduleToSave = _mapper.Map<ModuleEntity>(newModule);

            var savedModule = await _moduleRepo.CreateModuleAsync(moduleToSave);
            return _mapper.Map<ModuleDto>(savedModule);
        }

        public async Task<ModuleDto?> DeleteModule(Guid moduleId)
        {
            var deletedModule = await _moduleRepo.DeleteModuleByIdAsync(moduleId);
            if (deletedModule == null)
            {
                return null;
            }
            return _mapper.Map<ModuleDto>(deletedModule);
        }

        public async Task<IEnumerable<ModuleDto>?> GetAllModules()
        {
            var modules = await _moduleRepo.GetModulesAsync();
            if (modules == null)
            {
                return null;
            }

            return _mapper.Map<IEnumerable<ModuleDto>>(modules);
        }

        public async Task<ModuleDto?> GetModuleById(Guid moduleId)
        {
            var foundModule = await _moduleRepo.GetModuleByIdAsync(moduleId);
            if (foundModule == null)
            {
                return null;
            }

            return _mapper.Map<ModuleDto>(foundModule);
        }

        public async Task<ModuleDto?> UpdateModule(Guid moduleId, UpdateModuleDto updateModule)
        {
            if (updateModule.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                throw new InvalidDateException("Start date cannot be in the past.", 400);
            }
            int result = updateModule.EndDate.CompareTo(updateModule.StartDate);
            if (result < 0 || result == 0)
            {
                throw new InvalidDateException("End date has to be in the future.", 400);
            }
            var moduleFromDb = await _moduleRepo.GetModuleByIdAsync(moduleId);
            if (moduleFromDb == null)
            {
                return null;
            }

            _mapper.Map(updateModule, moduleFromDb);
            var updatedModule = await _moduleRepo.UpdateModuleAsync(moduleFromDb);

            return _mapper.Map<ModuleDto>(updatedModule);
        }
    }
}
