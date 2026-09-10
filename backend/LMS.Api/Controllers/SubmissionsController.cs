
using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SubmissionsController(ISubmissionsService _submissionsService) : ControllerBase
{

    /// <summary>
    /// Gets all submissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all submissions.</returns>
    /// 
    [HttpGet]
    [ProducesResponseType(typeof(List<SubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<List<SubmissionDto>>> GetAll(CancellationToken cancellationToken)
    {
        List<SubmissionDto> resources = await _submissionsService.GetAllAsync(cancellationToken);

        return Ok(resources);
    }

    /// <summary>
    /// Gets a paginated, searchable, sortable page of submissions.
    /// </summary>
    /// <param name="query">Search, sort, and paging options.</param>
    /// <param name="reviewStatus">Filter to submissions with this review status. Omit for no filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>One page of submissions.</returns>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<SubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<PagedResponse<SubmissionDto>>> GetPaged(
        [FromQuery] QueryParametersDto query,
        [FromQuery] SubmissionReviewStatus? reviewStatus,
        CancellationToken cancellationToken)
    {
        PagedResponse<SubmissionDto> resources = await _submissionsService.GetPagedAsync(query, reviewStatus, cancellationToken);

        return Ok(resources);
    }

    /// <summary>
    /// Gets the currently logged in students submissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Submissions created by the currently logged in student.</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(List<SubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<ActionResult<List<SubmissionDto>>> GetMe(CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out Guid studentId))
        {
            return Unauthorized();
        }

        List<SubmissionDto> submission = await _submissionsService.GetByStudentIdAsync(studentId, cancellationToken);

        return Ok(submission);
    }

    /// <summary>
    /// Creates a submission.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="submissionCreateDto">Requires activity ID and text content.</param>
    /// 
    [HttpPost]
    [ProducesResponseType(typeof(SubmissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<ActionResult<SubmissionDto>> CreateSubmission([FromBody] SubmissionCreateDto submissionCreateDto, CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out Guid studentId))
        {
            return Unauthorized();
        }
        SubmissionsCreateCommand command = new()
        {
            StudentId = studentId,
            ActivityId = submissionCreateDto.ActivityId,
            Text = submissionCreateDto.Text
        };

        SubmissionDto submission = await _submissionsService.CreateSubmission(command, cancellationToken);
        return Ok(submission);
    }

    /// <summary>
    /// Updates the submission with feedback from a teacher.
    /// </summary>
    /// <param name="submissionId">Submission Id.</param>
    /// <param name="feedbackDto">The required feedback text.</param>
    /// <param name="cancellationToken">The required feedback text.</param>

    [HttpPut("{submissionId:guid}/feedback")]
    [ProducesResponseType(typeof(SubmissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult> SetFeedback([FromRoute] Guid submissionId,
       [FromBody] SubmissionFeedbackDto feedbackDto, CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out Guid teacherId))
        {
            return Unauthorized();
        }

        SetFeedbackCommand command = new()
        {
            SubmissionId = submissionId,
            TeacherId = teacherId,
            Details = feedbackDto,
        };

        SubmissionDto? submission = await _submissionsService.SetFeedbackAsync(command, cancellationToken);
        return submission is null ? NotFound() : Ok(submission);
    }

    /// <summary>
    /// Gets a submission by submission ID.
    /// </summary>
    /// <param name="submissionId">The submission ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A specific submission based on a submission ID.</returns>
    [HttpGet("{submissionId:guid}")]
    [ProducesResponseType(typeof(SubmissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    public async Task<ActionResult<SubmissionDto>> GetById([FromRoute] Guid submissionId, CancellationToken cancellationToken)
    {
        string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out Guid userId))
        {
            return Unauthorized();
        }

        SubmissionDto? submission = await _submissionsService.GetByIdAsync(submissionId, cancellationToken);

        if (submission != null)
        {
            if (User.IsInRole(RoleConstants.Student) && userId != submission.StudentId)
            {
                return Forbid();
            }
        }

        if (submission is null)
        {
            return NotFound();
        }

        return Ok(submission);
    }

    /// <summary>
    /// Gets all submissions belonging to an activity.
    /// </summary>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of submissions belonging to an activity.</returns>
    [ProducesResponseType(typeof(List<SubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize]
    [HttpGet("activity/{activityId:guid}")]
    public async Task<ActionResult<List<SubmissionDto>>> GetByActivityIdAsync([FromRoute] Guid activityId, CancellationToken cancellationToken = default)
    {
        if (User.IsInRole(RoleConstants.Student))
        {
            return Forbid();
        }
        List<SubmissionDto> submissions = await _submissionsService.GetByActivityIdAsync(activityId, cancellationToken);

        if (submissions.Count == 0)
        {
            return NotFound();
        }

        return Ok(submissions);
    }

    /// <summary>
    /// Gets students enrolled in the activity's course who are overdue.
    /// </summary>
    /// <param name="activityId">The activity ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [ProducesResponseType(typeof(List<OverdueSubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = RoleConstants.Teacher)]
    [HttpGet("activity/{activityId:guid}/overdue")]
    public async Task<ActionResult<List<OverdueSubmissionDto>>> GetOverdueByActivityIdAsync([FromRoute] Guid activityId, CancellationToken cancellationToken = default)
    {
        List<OverdueSubmissionDto> overdue =
            await _submissionsService.GetOverdueByActivityIdAsync(activityId, cancellationToken);

        return Ok(overdue);
    }
}
