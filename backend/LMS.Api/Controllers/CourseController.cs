using LMS.Api.Models;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Api.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CourseController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseServce = courseService;

        /// <summary>
        /// Retrives a full list of all availbable courses
        /// </summary>
        /// <returns>The list of courses</returns>
        /// <response code="200">Returns the list of courses</response>
        /// <response code="404">If the list is not found</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _courseServce.GetAllCourses();
            if (courses == null)
            {
                return NotFound();
            }

            return Ok(courses);
        }
    }
}
