using LMS.Api.Data;
using LMS.Api.DTOs.Common;
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

    public async Task<PagedResponse<Course>> GetCoursesAsync(
        QueryParametersDto query,
        CancellationToken cancellationToken = default
    )
    {
        IQueryable<Course> coursesQuery = _context
            .Courses.AsNoTracking()
            .Include(course => course.Modules);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = query.Search.Trim();

            coursesQuery = coursesQuery.Where(course =>
                course.Name.Contains(search) || course.Description.Contains(search)
            );
        }

        coursesQuery = query.SortBy.ToLowerInvariant() switch
        {
            "startdate" => query.Direction == "desc"
                ? coursesQuery.OrderByDescending(course => course.StartDate)
                : coursesQuery.OrderBy(course => course.StartDate),

            "enddate" => query.Direction == "desc"
                ? coursesQuery.OrderByDescending(course => course.EndDate)
                : coursesQuery.OrderBy(course => course.EndDate),

            _ => query.Direction == "desc"
                ? coursesQuery.OrderByDescending(course => course.Name)
                : coursesQuery.OrderBy(course => course.Name),
        };

        int totalCount = await coursesQuery.CountAsync(cancellationToken);

        List<Course> courses = await coursesQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Course>
        {
            Items = courses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<IEnumerable<Module>> GetModulesForCourseAsync(Guid courseId)
    {
        return await _context
            .Modules.Include(m => m.Course)
            .Where(m => m.CourseId == courseId)
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
