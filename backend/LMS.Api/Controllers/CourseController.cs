using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Errors;
using LMS.Api.DTOs.Module;
using LMS.Api.Exceptions;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Route("api/courses")]
[ApiController]
[Authorize]
public class CourseController(ICourseService courseService, IEnrollmentService enrollmentService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;
    private readonly IEnrollmentService _enrollmentService = enrollmentService;
    /// <summary>
    /// Gets a paginated list of courses with optional search and sorting.
    /// </summary>
    /// <param name="query">
    /// Query parameters for search, sorting, page number, and page size.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token for the request.
    /// </param>
    /// <returns>
    /// A paginated list of courses.
    /// </returns>
    [HttpGet]
    [ProducesResponseType(
        typeof(PagedResponse<CourseDto>),
        StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<CourseDto>>> GetCourses([FromQuery] QueryParametersDto query, CancellationToken cancellationToken = default)
    {
        PagedResponse<CourseDto> courses =
            await _courseService.GetAllCourses(
                query,
                cancellationToken);

        return Ok(courses);
    }

    /// <summary>
    /// Retrieves a course based on CourseId.
    /// </summary>
    /// <param name="courseId">The ID of the course to retrieve,</param>
    /// <returns>The requested course,</returns>
    /// <response code="200">Returns the requested course.</response>
    /// <response code="404">If the course is not found.</response>
    [HttpGet("{courseId}", Name = "GetCourseById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetCourseById([FromRoute] Guid courseId)
    {
        ActionResult? accessResult = await ValidateCourseAccessAsync(courseId);

        if (accessResult is not null)
        {
            return accessResult;
        }
        var course = await _courseService.GetCourseById(courseId);
        if (course is null)
        {
            return NotFound();
        }

        return course;
    }

    /// <summary>
    /// Retrieves modules tied to a specific course..
    /// </summary>
    /// <param name="courseId">The ID of course whose modules are fetched.</param>
    /// <returns>A list of modules tied to a course.</returns>
    /// <response code="200">Returns the requested modules.</response>
    /// <response code="404">If the course is not found.</response>
    [HttpGet("{courseId}/get-modules")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesForCourse(
        [FromRoute] Guid courseId
    )
    {

        ActionResult? accessResult = await ValidateCourseAccessAsync(courseId);

        if (accessResult is not null)
        {
            return accessResult;
        }

        var modules = await _courseService.GetModulesForCourse(courseId);
        if (modules == null)
        {
            return NotFound();
        }

        return Ok(modules);
    }

    /// <summary>
    /// Create a new course.
    /// </summary>
    /// <param name="newCourseDto">Contains the required fields for creating a new course. Name, Descrtiption, Start and End-date.</param>
    /// <returns>The saved course.</returns>
    /// <response code="201">Successfully created course, and returns the newly created course.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<CourseDto>> CreateNewCourse(
        [FromBody] CreateNewCourseDto newCourseDto
    )
    {
        try
        {
            var savedCourse = await _courseService.CreateNewCourse(newCourseDto);
            return CreatedAtAction(
                "GetCourseById",
                new { courseId = savedCourse.CourseId },
                savedCourse
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
    /// Update an existing course.
    /// </summary>
    /// <param name="courseId">The Id of the course to update.</param>
    /// <param name="updateCourseDto">Contains the required fields for updating a course. Name, Descrtiption, Start and End-date.</param>
    /// <returns>The updated course.</returns>
    /// <response code="200">Course was successfully updated and returned.</response>
    [HttpPut("{courseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<ActionResult<CourseDto>> UpdateCourse(
        [FromRoute] Guid courseId,
        [FromBody] UpdateCourseDto updateCourseDto
    )
    {
        try
        {
            var updatedCourse = await _courseService.UpdateCourse(courseId, updateCourseDto);
            return Ok(updatedCourse);
        }
        catch (ArgumentException exc)
        {
            return BadRequest(exc.Message);
        }
    }

    /// <summary>
    /// Delete an existing course.
    /// </summary>
    /// <param name="courseId">The Id of the course to delete.</param>
    /// <response code="204">Course was successfully deleted.</response>
    /// <response code="404">Course was not found.</response>
    [HttpDelete("{courseId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = RoleConstants.Teacher)]
    public async Task<IActionResult> DeleteCourse([FromRoute] Guid courseId)
    {
        var deletedCourse = await _courseService.DeleteCourse(courseId);
        if (deletedCourse == null)
        {
            return NotFound();
        }

        return NoContent();
    }

    private async Task<ActionResult?> ValidateCourseAccessAsync(Guid courseId)
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

        if (studentCourse.CourseId != courseId)
        {
            return Forbid();
        }

        return null;
    }
}
