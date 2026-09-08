using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly LMSDbContext _context;

    public EnrollmentRepository(LMSDbContext context)
    {
        _context = context;
    }

    public async Task<Enrollment?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Enrollments
            .Include(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(
                enrollment => enrollment.StudentId == studentId,
                cancellationToken);
    }

    public async Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
    }

    public void Update(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
    }
}
