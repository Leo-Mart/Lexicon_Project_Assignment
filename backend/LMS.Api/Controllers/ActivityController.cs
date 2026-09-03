
using LMS.Api.DTOs.Activities;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Tags("Activities")]
public class ActivityController : ControllerBase
{

    private readonly IActivityService _activityService;

    public ActivityController(IActivityService activityService)
    {
        _activityService = activityService;
    }

    /// <summary>
    /// Gets all activities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of activities.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ActivityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActivityDto>>> GetAllActivities(CancellationToken cancellationToken)
    {
        List<ActivityDto> activities = await _activityService.GetAllAsync(cancellationToken);

        return Ok(activities);
    }

    /// <summary>
    /// Gets an activity by ID.
    /// </summary>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested activity.</returns>
    [HttpGet("{activityId:guid}")]
    [ProducesResponseType(typeof(ActivityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActivityDto?>> GetActivityByIdAsync([FromRoute] Guid activityId, CancellationToken cancellationToken = default)
    {
        ActivityDto? activity = await _activityService.GetByIdAsync(activityId, cancellationToken);

        if (activity is null)
        {
            return NotFound();
        }

        return Ok(activity);
    }

    /// <summary>
    /// Gets all activities belonging to a module.
    /// </summary>
    /// <param name="moduleId">The module ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of activities belonging to the module.</returns>
    [HttpGet("module/{moduleId:guid}")]
    public async Task<ActionResult<List<ActivityDto>>> GetActivitiesByModuleIdAsync([FromRoute] Guid moduleId, CancellationToken cancellationToken = default)
    {
        List<ActivityDto> activities = await _activityService.GetByModuleIdAsync(moduleId, cancellationToken);

        return Ok(activities);
    }

    /// <summary>
    /// Deletes an activity.
    /// </summary>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{activityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteActivityAsync([FromRoute] Guid activityId, CancellationToken cancellationToken = default)
    {
        bool deleted = await _activityService.DeleteAsync(activityId, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Updates an existing activity.
    /// </summary>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="request">Updated activity information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{activityId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateActivityAsyny([FromRoute] Guid activityId, [FromBody] ActivityUpdateDto request, CancellationToken cancellationToken)
    {
        bool updated = await _activityService.UpdateAsync(activityId, request, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Creates a new activity.
    /// </summary>
    /// <param name="request">The activity information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The newly created activity.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ActivityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateActivityAsync([FromBody] ActivityCreateDto request, CancellationToken cancellationToken = default)
    {
        ActivityDto activity = await _activityService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetActivityByIdAsync),
            new { activityId = activity.ActivityId },
            activity
        );
    }
}

