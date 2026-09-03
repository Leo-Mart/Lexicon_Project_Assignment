using AutoMapper;
using LMS.Api.DTOs.Module;
using LMS.Api.Exceptions;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;
using LMS.Api.Validators;

namespace LMS.Api.Services.Implementations;

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
        await ValidateModuleDatesAsync(
            newModule.CourseId,
            newModule.StartDate,
            newModule.EndDate,
            validateNotBefore: true
        );

        var course = await _courseRepo.GetCourseByIdAsync(newModule.CourseId);
        if (course == null)
        {
            throw new ArgumentException("Could not find course.");
        }

        var moduleToSave = _mapper.Map<Module>(newModule);

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
        var moduleFromDb = await _moduleRepo.GetModuleByIdAsync(moduleId);
        if (moduleFromDb == null)
        {
            return null;
        }

        await ValidateModuleDatesAsync(
            moduleFromDb.CourseId,
            updateModule.StartDate,
            updateModule.EndDate,
            excludedModuleId: moduleId
        );

        _mapper.Map(updateModule, moduleFromDb);
        var updatedModule = await _moduleRepo.UpdateModuleAsync(moduleFromDb);

        return _mapper.Map<ModuleDto>(updatedModule);
    }

    private async Task ValidateModuleDatesAsync(
        Guid courseId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludedModuleId = null,
        bool validateNotBefore = false
    )
    {
        DateRangeValidator.ValidateRange(startDate, endDate, "Module");

        if (validateNotBefore)
        {
            DateRangeValidator.ValidateNotBefore(
                startDate,
                DateOnly.FromDateTime(DateTime.UtcNow),
                "Module"
            );
        }

        Course? course = await _courseRepo.GetCourseByIdAsync(courseId);

        if (course is null)
        {
            throw new KeyNotFoundException("Course not found.");
        }

        DateRangeValidator.ValidateWithinParent(
            startDate,
            endDate,
            course.StartDate,
            course.EndDate,
            "Module",
            "Course"
        );

        foreach (Module existingModule in course.Modules)
        {
            if (
                excludedModuleId.HasValue
                && existingModule.ModuleId == excludedModuleId.Value
            )
            {
                continue;
            }

            bool overlaps = DateRangeValidator.Overlaps(
                startDate,
                endDate,
                existingModule.StartDate,
                existingModule.EndDate
            );

            if (overlaps)
            {
                throw new OverlappingDateException(
                    $"Module overlaps with existing module: {existingModule.Name}.",
                    400
                );
            }
        }
    }
}
