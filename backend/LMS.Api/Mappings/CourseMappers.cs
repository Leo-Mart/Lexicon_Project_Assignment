using LMS.Api.DTOs.Courses;
using LMS.Api.Models;

namespace LMS.Api.Mappings
{
    public static class CourseMappers
    {
        public static CourseDto ToDtoFromCourse(this Course courseModel)
        {
            return new CourseDto
            {
                CourseId = courseModel.CourseId,
                Name = courseModel.Name,
                Description = courseModel.Description,
                StartDate = courseModel.StartDate,
                EndDate = courseModel.EndDate,
            };
        }
    }
}
