using LMS.Api.DTOs.Courses;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;

        /// <summary>
        /// Retrieves a full list of all available courses
        /// </summary>
        /// <returns>The list of courses</returns>
        /// <response code="200">Returns the list of courses</response>
        /// <response code="404">If the list is not found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
        {
            var courses = await _courseService.GetAllCourses();
            if (courses == null)
            {
                return NotFound();
            }

            return Ok(courses);
        }

        /// <summary>
        /// Retrieves a course based on CourseId
        /// </summary>
        /// <param name="courseId">The ID of the course to retrieve</param>
        /// <returns>The requested course</returns>
        /// <response code="200">Returns the requested course</response>
        /// <response code="404">If the course is not found</response>
        [HttpGet("{courseId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDto>> GetCourseById([FromRoute] Guid courseId)
        {
            Console.WriteLine(courseId);
            var course = await _courseService.GetCourseById(courseId);
            if (course == null)
            {
                return NotFound();
            }

            return course;
        }
    }
}
