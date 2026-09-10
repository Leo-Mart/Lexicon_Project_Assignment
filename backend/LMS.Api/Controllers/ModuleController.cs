using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Errors;
using LMS.Api.DTOs.Module;
using LMS.Api.Exceptions;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Route("/api/modules")]
[ApiController]
[Authorize]
public class ModuleController(IModuleService moduleService, IEnrollmentService enrollmentService) : ControllerBase
{
    private readonly IModuleService _moduleService = moduleService;
    private readonly IEnrollmentService _enrollmentService = enrollmentService;

    /// <summary>
    /// Retrieves a full list of all available modules.
    /// </summary>
    /// <returns>The list of modules.</returns>
    /// <response code="200">Returns the list of modules.</response>
    /// <response code="404">If the list is not found.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules()
    {
        var modules = await _moduleService.GetAllModules();
        if (modules == null)
        {
            return NotFound();
        }

        return Ok(modules);
    }

    /// <summary>
    /// Retrieves a module based on moduleId.
    /// </summary>
    /// <param name="moduleId">The ID of the module to retrieve,</param>
    /// <returns>The requested module,</returns>
    /// <response code="200">Returns the requested module.</response>
    /// <response code="404">If the module is not found.</response>
    [HttpGet("{moduleId}", Name = "GetModuleById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ModuleDto>> GetModuleById([FromRoute] Guid moduleId)
    {
        var module = await _moduleService.GetModuleById(moduleId);
        if (module == null)
        {
            return NotFound();
        }

        ActionResult? accessResult = await ValidateModuleAccessAsync(module);

        if (accessResult is not null)
        {
            return accessResult;
        }

        return module;
    }

    /// <summary>
    /// Create a new module.
    /// </summary>
    /// <param name="newModuleDto">Contains the required fields for creating a new module. Name, Descrtiption, Start and End-date.</param>
    /// <returns>The saved module.</returns>
    /// <response code="201">Successfully created module, and returns the newly created module.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<ModuleDto>> CreateNewModule(
        [FromBody] CreateNewModuleDto newModuleDto
    )
    {
        try
        {
            var savedModule = await _moduleService.CreateNewModule(newModuleDto);
            return CreatedAtAction(
                "GetmoduleById",
                new { moduleId = savedModule.ModuleId },
                savedModule
            );
        }
        catch (InvalidDateException exc)
        {
            return BadRequest(
                new ErrorResponse { Message = exc.Message, StatusCode = exc.StatusCode }
            );
        }
        catch (OverlappingDateException exc)
        {
            return BadRequest(
                new ErrorResponse { Message = exc.Message, StatusCode = exc.StatusCode }
            );
        }
    }

    /// <summary>
    /// Update an existing module.
    /// </summary>
    /// <param name="moduleId">The Id of the module to update.</param>
    /// <param name="updateModuleDto">Contains the required fields for updating a module. Name, Descrtiption, Start and End-date.</param>
    /// <returns>The updated module.</returns>
    /// <response code="200">module was successfully updated and returned.</response>
    [HttpPut("{moduleId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<ModuleDto>> UpdateModule(
        [FromRoute] Guid moduleId,
        [FromBody] UpdateModuleDto updateModuleDto
    )
    {
        var updatedmodule = await _moduleService.UpdateModule(moduleId, updateModuleDto);
        return Ok(updatedmodule);
    }

    /// <summary>
    /// Delete an existing module.
    /// </summary>
    /// <param name="moduleId">The Id of the module to delete.</param>
    /// <response code="204">module was successfully deleted.</response>
    /// <response code="404">module was not found.</response>
    [HttpDelete("{moduleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<IActionResult> Deletemodule([FromRoute] Guid moduleId)
    {
        var deletedmodule = await _moduleService.DeleteModule(moduleId);
        if (deletedmodule == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    private async Task<ActionResult?> ValidateModuleAccessAsync(ModuleDto module)
    {
        if (!User.IsInRole(RoleConstants.Student))
        {
            return null;
        }

        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out Guid studentId))
        {
            return Unauthorized();
        }

        CourseDto? studentCourse = await _enrollmentService.GetStudentCourseAsync(studentId);

        if (studentCourse is null)
        {
            return NotFound();
        }

        if (module.CourseId != studentCourse.CourseId)
        {
            return Forbid();
        }

        return null;
    }
}
