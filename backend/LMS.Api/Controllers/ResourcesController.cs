using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Module;
using LMS.Api.DTOs.Resources;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Tags("Resources")]
public class ResourcesController : ControllerBase
{
    private readonly IResourceService _resourceService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IModuleService _moduleService;
    private readonly IActivityService _activityService;

    public ResourcesController(
        IResourceService resourceService,
        IEnrollmentService enrollmentService,
        IModuleService moduleService,
        IActivityService activityService)
    {
        _resourceService = resourceService;
        _enrollmentService = enrollmentService;
        _moduleService = moduleService;
        _activityService = activityService;
    }

    /// <summary>
    /// Gets a paginated list of resources with optional search and sorting.
    /// </summary>
    /// <param name="query">
    /// Query parameters for search, sorting, page number, and page size.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the request.
    /// </param>
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponse<ResourceDto>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<PagedResponse<ResourceDto>>> GetAll([FromQuery] QueryParametersDto query, CancellationToken cancellationToken)
    {
        PagedResponse<ResourceDto> resources = await _resourceService.GetAllAsync(query, cancellationToken);

        return Ok(resources);
    }

    /// <summary>
    /// Gets a resource by ID.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResourceDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        ResourceDto? resource = await _resourceService.GetByIdAsync(id, cancellationToken);

        if (resource is null)
        {
            return NotFound();
        }

        return Ok(resource);
    }

    /// <summary>
    /// Gets all resources connected to a course.
    /// </summary>
    /// <param name="courseId">The course ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(List<ResourceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ResourceDto>>> GetByCourseId([FromRoute] Guid courseId, CancellationToken cancellationToken)
    {
        ActionResult? accessResult = await ValidateCourseAccessAsync(courseId, cancellationToken);

        if (accessResult is not null)
        {
            return accessResult;
        }
        List<ResourceDto> resources = await _resourceService.GetByCourseIdAsync(courseId, cancellationToken);

        return Ok(resources);
    }

    /// <summary>
    /// Gets all resources connected to a module.
    /// </summary>
    /// <param name="moduleId">The module ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("module/{moduleId:guid}")]
    [ProducesResponseType(typeof(List<ResourceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ResourceDto>>> GetByModuleId([FromRoute] Guid moduleId, CancellationToken cancellationToken)
    {
        ActionResult? accessResult = await ValidateModuleAccessAsync(moduleId, cancellationToken);

        if (accessResult is not null)
        {
            return accessResult;
        }
        List<ResourceDto> resources = await _resourceService.GetByModuleIdAsync(moduleId, cancellationToken);

        return Ok(resources);
    }

    /// <summary>
    /// Gets all resources connected to an activity.
    /// </summary>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [HttpGet("activity/{activityId:guid}")]
    [ProducesResponseType(typeof(List<ResourceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<ResourceDto>>> GetByActivityId([FromRoute] Guid activityId, CancellationToken cancellationToken)
    {
        ActionResult? accessResult = await ValidateActivityAccessAsync(activityId, cancellationToken);

        if (accessResult is not null)
        {
            return accessResult;
        }

        List<ResourceDto> resources = await _resourceService.GetByActivityIdAsync(activityId, cancellationToken);

        return Ok(resources);
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="request">The resource information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpPost]
    [ProducesResponseType(typeof(ResourceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ResourceDto>> Create([FromBody] ResourceCreateDto request, CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out Guid teacherId))
        {
            return Unauthorized();
        }

        ResourceDto resource = await _resourceService.CreateAsync(teacherId, request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = resource.ResourceId }, resource);
    }

    /// <summary>
    /// Updates an existing resource.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <param name="request">Updated resource information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] ResourceUpdateDto request, CancellationToken cancellationToken)
    {
        bool updated = await _resourceService.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a resource.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        bool deleted = await _resourceService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Connects a resource to a course.
    /// </summary>
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpPost("{resourceId:guid}/course/{courseId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToCourse(
        [FromRoute] Guid resourceId,
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        bool added = await _resourceService.AddToCourseAsync(resourceId, courseId, cancellationToken);

        if (!added)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Connects a resource to a module.
    /// </summary>
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpPost("{resourceId:guid}/module/{moduleId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToModule(
        [FromRoute] Guid resourceId,
        [FromRoute] Guid moduleId,
        CancellationToken cancellationToken)
    {
        bool added = await _resourceService.AddToModuleAsync(resourceId, moduleId, cancellationToken);

        if (!added)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Connects a resource to an activity.
    /// </summary>
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpPost("{resourceId:guid}/activity/{activityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddToActivity(
        [FromRoute] Guid resourceId,
        [FromRoute] Guid activityId,
        CancellationToken cancellationToken)
    {
        bool added = await _resourceService.AddToActivityAsync(resourceId, activityId, cancellationToken);

        if (!added)
        {
            return NotFound();
        }

        return NoContent();
    }

    private async Task<ActionResult?> ValidateCourseAccessAsync(Guid courseId, CancellationToken cancellationToken)
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

        CourseDto? studentCourse =
            await _enrollmentService.GetStudentCourseAsync(studentId, cancellationToken);

        if (studentCourse is null)
        {
            return NotFound();
        }

        if (studentCourse.CourseId != courseId)
        {
            return Forbid();
        }

        return null;
    }

    private async Task<ActionResult?> ValidateModuleAccessAsync(Guid moduleId, CancellationToken cancellationToken)
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

        CourseDto? studentCourse = await _enrollmentService.GetStudentCourseAsync(studentId, cancellationToken);

        if (studentCourse is null)
        {
            return NotFound();
        }

        ModuleDto? module = await _moduleService.GetModuleById(moduleId);

        if (module is null)
        {
            return NotFound();
        }

        if (module.CourseId != studentCourse.CourseId)
        {
            return Forbid();
        }

        return null;
    }

    private async Task<ActionResult?> ValidateActivityAccessAsync(Guid activityId, CancellationToken cancellationToken)
    {
        if (!User.IsInRole(RoleConstants.Student))
        {
            return null;
        }

        ActivityDto? activity = await _activityService.GetByIdAsync(activityId, cancellationToken);

        if (activity is null)
        {
            return NotFound();
        }

        return await ValidateModuleAccessAsync(activity.ModuleId, cancellationToken);
    }
}
