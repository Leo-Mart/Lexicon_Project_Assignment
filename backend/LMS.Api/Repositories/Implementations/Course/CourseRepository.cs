using LMS.Api.Data;
using LMS.Api.DTOs.Course;
using LMS.Api.Repositories.Interfaces.Course;
using Microsoft.EntityFrameworkCore;
using CourseEntity = LMS.Api.Models.Course;

namespace LMS.Api.Repositories.Implementations.Course
{
    public class CourseRepository(LMSDbContext context) : ICourseRepository
    {
        private readonly LMSDbContext _context = context;

        public async Task<CourseEntity> CreateCourseAsync(CourseEntity course)
        {
            course.CreatedAt = DateTime.UtcNow;
            course.UpdatedAt = DateTime.UtcNow;

            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return course;
        }

        public async Task<CourseEntity?> DeleteCourseByIdAsync(Guid courseId)
        {
            var foundCourse = await _context.Courses.FirstOrDefaultAsync(c =>
                c.CourseId == courseId
            );

            if (foundCourse == null)
            {
                return null;
            }

            _context.Courses.Remove(foundCourse);
            await _context.SaveChangesAsync();
            return foundCourse;
        }

        public async Task<CourseEntity?> GetCourseByIdAsync(Guid courseId)
        {
            return await _context
                .Courses.Include(c => c.Modules)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<IEnumerable<CourseEntity>> GetCoursesAsync()
        {
            return await _context.Courses.Include(c => c.Modules).ToListAsync();
        }

        public async Task<CourseEntity?> UpdateCourseAsync(Guid courseId, UpdateCourseDto updateDto)
        {
            var courseFromDb = await _context.Courses.FirstOrDefaultAsync(c =>
                c.CourseId == courseId
            );
            if (courseFromDb == null)
            {
                return null;
            }

            courseFromDb.Name = updateDto.Name;
            courseFromDb.Description = updateDto.Description;
            courseFromDb.StartDate = updateDto.StartDate;
            courseFromDb.EndDate = updateDto.EndDate;

            await _context.SaveChangesAsync();
            return courseFromDb;
        }
    }
}
