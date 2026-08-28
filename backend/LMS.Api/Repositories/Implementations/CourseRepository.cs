using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations
{
    public class CourseRepository(LMSDbContext context) : ICourserepository
    {
        private readonly LMSDbContext _context = context;

        public Task<Course> CreateCourseAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<Course> GetCourseByIdAsync(Guid courseId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }
    }
}
