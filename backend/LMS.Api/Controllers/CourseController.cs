using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Course;
using LMS.Api.DTOs.Errors;
using LMS.Api.Exceptions;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers;

[Route("api/courses")]
[ApiController]
public class CourseController(ICourseService courseService) : ControllerBase
{
    private readonly ICourseService _courseService = courseService;
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
        var course = await _courseService.GetCourseById(courseId);
        if (course == null)
        {
            return NotFound();
        }

        return course;
    }

    /// <summary>
    /// Create a new course.
    /// </summary>
    /// <param name="newCourseDto">Contains the required fields for creating a new course. Name, Descrtiption, Start and End-date.</param>
    /// <returns>The saved course.</returns>
    /// <response code="201">Successfully created course, and returns the newly created course.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
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
    public async Task<IActionResult> DeleteCourse([FromRoute] Guid courseId)
    {
        var deletedCourse = await _courseService.DeleteCourse(courseId);
        if (deletedCourse == null)
        {
            return NotFound();
        }

        return NoContent();
    }
}
