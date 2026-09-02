
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
    /// Gets a submission by ID.
    /// </summary>
    /// <param name="id">The resource ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// 
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubmissionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<SubmissionDto>> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        SubmissionDto? submission = await _submissionsService.GetByIdAsync(id, cancellationToken);

        if (submission is null)
        {
            return NotFound();
        }

        return Ok(submission);
    }
}
