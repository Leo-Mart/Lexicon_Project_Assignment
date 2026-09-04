using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class CourseRepository(LMSDbContext context) : ICourseRepository
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

    public async Task<Course?> DeleteCourseByIdAsync(Guid courseId)
    {
        var foundCourse = await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);

        if (foundCourse == null)
        {
            return null;
        }

        _context.Courses.Remove(foundCourse);
        await _context.SaveChangesAsync();
        return foundCourse;
    }

    public async Task<Course?> GetCourseByIdAsync(Guid courseId)
    {
        return await _context
            .Courses.Include(c => c.Modules)
            .FirstOrDefaultAsync(c => c.CourseId == courseId);
    }

    public async Task<IEnumerable<Course>> GetCoursesAsync()
    {
        return await _context.Courses
        .Include(c => c.Modules)
        .OrderBy(course => course.StartDate)
        .ToListAsync();
    }

    public async Task<Course> UpdateCourseAsync(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;

        _context.Courses.Update(course);
        await _context.SaveChangesAsync();

        return course;
    }
}
