using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations
{
    public class CourseRepository(LMSDbContext context) : ICourserepository
    {
        private readonly LMSDbContext _context = context;

        public async Task<Course> CreateCourseAsync(Course course)
        {
            course.CreatedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return course;
        }

        public async Task<Course?> GetCourseByIdAsync(Guid courseId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return await _context.Courses.ToListAsync();
        }
    }
}
