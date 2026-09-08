
using System.Runtime.CompilerServices;
using LMS.Api.DTOs.Submissions;

namespace LMS.Api.Services.Interfaces;

public interface ISubmissionsService
{
    Task<SubmissionDto?> SetFeedbackAsync(SetFeedbackCommand setFeedbackCommand, CancellationToken cancellationToken = default);
    Task<List<SubmissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<SubmissionDto>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    Task<List<SubmissionDto>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<List<OverdueSubmissionDto>> GetOverdueByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default);
    Task<SubmissionDto?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<SubmissionDto> CreateSubmission(SubmissionsCreateCommand command, CancellationToken cancellationToken);
}
