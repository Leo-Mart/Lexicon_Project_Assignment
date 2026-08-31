using LMS.Api.DTOs.Module;
using LMS.Api.Mappings;
using LMS.Api.Repositories.Interfaces.Course;
using LMS.Api.Repositories.Interfaces.Module;
using LMS.Api.Services.Interfaces.Module;

namespace LMS.Api.Services.Implementations
{
    public class ModuleService(IModuleRepository moduleRepo, ICourseRepository courseRepo)
        : IModuleService
    {
        private readonly IModuleRepository _moduleRepo = moduleRepo;
        private readonly ICourseRepository _courseRepo = courseRepo;

        public async Task<ModuleDto> CreateNewModule(CreateNewModuleDto newModule)
        {
            if (newModule.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                throw new ArgumentException("Start date cannot be in the past.");
            }
            int result = newModule.EndDate.CompareTo(newModule.StartDate);
            if (result < 0 || result == 0)
            {
                throw new ArgumentException("End date has to be in the future.");
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

                Console.WriteLine(overlaps);

                if (overlaps)
                {
                    throw new ArgumentException(
                        $"Could not create module. Dates overlap with existing module: {module.Name}"
                    );
                }
            }

            var courseToSave = newModule.ToModuleFromCreate();

            var savedModule = await _moduleRepo.CreateModuleAsync(courseToSave);
            return savedModule.ToDtoFromModule();
        }

        public async Task<ModuleDto?> DeleteModule(Guid moduleId)
        {
            var deletedCourse = await _moduleRepo.DeleteModuleByIdAsync(moduleId);
            if (deletedCourse == null)
            {
                return null;
            }
            return deletedCourse.ToDtoFromModule();
        }

        public async Task<IEnumerable<ModuleDto>?> GetAllModules()
        {
            var modules = await _moduleRepo.GetModulesAsync();
            if (modules == null)
            {
                return null;
            }

            return modules.Select(m => m.ToDtoFromModule());
        }

        public async Task<ModuleDto?> GetModuleById(Guid moduleId)
        {
            var foundModule = await _moduleRepo.GetModuleByIdAsync(moduleId);
            if (foundModule == null)
            {
                return null;
            }

            return foundModule.ToDtoFromModule();
        }

        public async Task<ModuleDto?> UpdateModule(Guid moduleId, UpdateModuleDto updateModule)
        {
            if (updateModule.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                throw new ArgumentException("Start date cannot be in the past.");
            }
            int result = updateModule.EndDate.CompareTo(updateModule.StartDate);
            if (result < 0 || result == 0)
            {
                throw new ArgumentException("End date has to be in the future.");
            }
            var updatedModule = await _moduleRepo.UpdateModuleAsync(moduleId, updateModule);
            if (updatedModule == null)
            {
                return null;
            }

            return updatedModule.ToDtoFromModule();
        }
    }
}
