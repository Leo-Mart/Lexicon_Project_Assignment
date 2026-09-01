using LMS.Api.DTOs.Module;
using LMS.Api.Services.Interfaces.Module;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers
{
    [Route("/api/modules")]
    [ApiController]
    public class ModuleController(IModuleService moduleService) : ControllerBase
    {
        private readonly IModuleService _moduleService = moduleService;

        /// <summary>
        /// Retrieves a full list of all available modules.
        /// </summary>
        /// <returns>The list of modules.</returns>
        /// <response code="200">Returns the list of modules.</response>
        /// <response code="404">If the list is not found.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            catch (ArgumentException exc)
            {
                return BadRequest(exc.Message);
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
        public async Task<IActionResult> Deletemodule([FromRoute] Guid moduleId)
        {
            var deletedmodule = await _moduleService.DeleteModule(moduleId);
            if (deletedmodule == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
