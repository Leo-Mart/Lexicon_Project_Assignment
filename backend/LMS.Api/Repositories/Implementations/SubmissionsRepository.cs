using LMS.Api.Data;
using LMS.Api.DTOs.Common;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LMS.Api.Repositories.Implementations;

public class SubmissionsRepository(LMSDbContext _context) : ISubmissionsRepository
{
    public async Task<List<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.AsNoTracking().Include(submission => submission.Activity).ToListAsync(cancellationToken);
    }

    public async Task<PagedResponse<Submission>> GetPagedAsync(QueryParametersDto query, SubmissionReviewStatus? reviewStatus = null, CancellationToken cancellationToken = default)
    {
        IQueryable<Submission> submissionsQuery = _context.Submissions
            .AsNoTracking()
            .Include(submission => submission.Activity)
                .ThenInclude(activity => activity.Module)
                    .ThenInclude(module => module.Course)
            .Include(submission => submission.Student);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            string search = query.Search.Trim();

            submissionsQuery = submissionsQuery.Where(submission =>
                submission.Student.Name.Contains(search));
        }

        // Null reviewStatus filters to not-yet-reviewed submissions, same as the model itself.
        submissionsQuery = submissionsQuery.Where(submission => submission.ReviewStatus == reviewStatus);

        submissionsQuery = query.SortBy.ToLowerInvariant() switch
        {
            "student" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission => submission.Student.Name)
                : submissionsQuery.OrderBy(submission => submission.Student.Name),

            "course" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission => submission.Activity.Module.Course.Name)
                : submissionsQuery.OrderBy(submission => submission.Activity.Module.Course.Name),

            "activity" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission => submission.Activity.Name)
                : submissionsQuery.OrderBy(submission => submission.Activity.Name),

            "submitted" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission => submission.SubmittedAt)
                : submissionsQuery.OrderBy(submission => submission.SubmittedAt),

            "deadline" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission => submission.Activity.Deadline)
                : submissionsQuery.OrderBy(submission => submission.Activity.Deadline),

            "late" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission =>
                    submission.Activity.Deadline != null && submission.SubmittedAt > submission.Activity.Deadline)
                : submissionsQuery.OrderBy(submission =>
                    submission.Activity.Deadline != null && submission.SubmittedAt > submission.Activity.Deadline),

            "review" => query.Direction == "desc"
                ? submissionsQuery.OrderByDescending(submission => submission.ReviewStatus)
                : submissionsQuery.OrderBy(submission => submission.ReviewStatus),

            // Not reviewed first by default - teachers open this page to find work waiting on them.
            _ => submissionsQuery.OrderBy(submission => submission.ReviewStatus)
        };

        int totalCount = await submissionsQuery.CountAsync(cancellationToken);

        List<Submission> submissions = await submissionsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<Submission>
        {
            Items = submissions,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<Submission?> GetByIdAsync(
        Guid resourceId,
        CancellationToken cancellationToken = default
    )
    {
        return await _context
            .Submissions.AsNoTracking()
            .Include(submission => submission.Activity)
            .FirstOrDefaultAsync(submission => submission.SubmissionId == resourceId, cancellationToken);
    }

    public async Task<List<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions.AsNoTracking().Include(submission => submission.Activity).Where(submission => submission.StudentId == studentId).ToListAsync(cancellationToken);
    }
    public void Update(Submission submission) => _context.Submissions.Update(submission);

    public async Task CreateAsync(Submission submission, CancellationToken cancellationToken)
    {
        await _context.AddAsync(submission);
    }

    public async Task<List<Submission>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken)
    {
        return await _context.Submissions.AsNoTracking().Include(submission => submission.Activity).Where(submission => submission.ActivityId == activityId).ToListAsync(cancellationToken);
    }
}
