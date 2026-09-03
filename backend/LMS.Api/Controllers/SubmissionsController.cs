
using System.Diagnostics;
using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Submissions;
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
    /// Gets the students submissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
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
    [ProducesResponseType(typeof(List<SubmissionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = RoleConstants.Student)]
    public async Task<ActionResult<List<SubmissionDto>>> CreateSubmission([FromBody] SubmissionCreateDto submissionCreateDto, CancellationToken cancellationToken)
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

        bool success = await _submissionsService.CreateSubmission(command, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Updates the submission with feedback from a teacher.
    /// </summary>
    /// <param name="submissionId">Submission Id.</param>
    /// <param name="feedbackDto">The required feedback text.</param>
    /// <param name="cancellationToken">The required feedback text.</param>
    /// 
    [HttpPut("{submissionId:guid}/feedback")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

        bool success = await _submissionsService.SetFeedbackAsync(command, cancellationToken);
        return success ? NoContent() : NotFound();
    }

    // /// <summary>
    // /// Gets a submission by ID.
    // /// </summary>
    // /// <param name="submissionId">The resource ID.</param>
    // /// <param name="cancellationToken">Cancellation token.</param>
    // /// 
    // [HttpGet("{id:guid}")]
    // [ProducesResponseType(typeof(SubmissionDto), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    // [ProducesResponseType(StatusCodes.Status403Forbidden)]
    // [ProducesResponseType(StatusCodes.Status404NotFound)]
    // [Authorize]
    // public async Task<ActionResult<SubmissionDto>> GetById([FromRoute] Guid submissionId, CancellationToken cancellationToken)
    // {
    //     //We need: Student ID, we need Activity ID. 
    //     string? userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //     if (!Guid.TryParse(userIdClaim, out Guid teacherId))
    //     {
    //         return Unauthorized();
    //     }

    //     SubmissionDto? submission = await _submissionsService.GetByIdAsync(submissionId, cancellationToken);

    //     if (submission is null)
    //     {
    //         return NotFound();
    //     }

    //     return Ok(submission);
    // }
}
