using LMS.Api.Data;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class SubmissionsRepository(LMSDbContext _context) : ISubmissionsRepository
{
    public async Task<List<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Submission?> GetByIdAsync(
        Guid resourceId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Submissions.AsNoTracking()
            .FirstOrDefaultAsync(submission => submission.SubmissionId == resourceId, cancellationToken);
    }
}
