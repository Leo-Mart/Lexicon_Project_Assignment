using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Course;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

/// <summary>
/// Handles course enrollments for students.
/// </summary>
[Route("api/enrollments")]
[ApiController]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Gets the course assigned to the currently authenticated student.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    /// <returns>The course assigned to the current student.</returns>
    [HttpGet("course")]
    [Authorize(Roles = RoleConstants.Student)]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetStudentCourse(
        CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out Guid studentId))
        {
            return Unauthorized();
        }

        CourseDto? course = await _enrollmentService.GetStudentCourseAsync(studentId, cancellationToken);

        if (course is null)
        {
            return NotFound();
        }

        return Ok(course);
    }

    /// <summary>
    /// Assigns a course to a student or changes the student's existing course.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="courseId">The ID of the course to assign.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [HttpPut("students/{studentId:guid}/course/{courseId:guid}")]
    [Authorize(Roles = RoleConstants.Teacher)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignOrChangeCourse(
        [FromRoute] Guid studentId,
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _enrollmentService.AssignOrChangeCourseAsync(
                studentId,
                courseId,
                cancellationToken);

            return NoContent();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    /// <summary>
    /// Removes the course assignment from a student.
    /// </summary>
    /// <param name="studentId">The ID of the student.</param>
    /// <param name="cancellationToken">Cancellation token for the request.</param>
    [HttpDelete("students/{studentId:guid}/course")]
    [Authorize(Roles = RoleConstants.Teacher)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveCourse(
        [FromRoute] Guid studentId,
        CancellationToken cancellationToken)
    {
        await _enrollmentService.RemoveCourseAsync(
            studentId,
            cancellationToken);

        return NoContent();
    }
}
