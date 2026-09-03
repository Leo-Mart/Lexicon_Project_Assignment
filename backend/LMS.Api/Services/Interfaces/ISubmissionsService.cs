
using System.Runtime.CompilerServices;
using LMS.Api.DTOs.Submissions;

namespace LMS.Api.Services.Interfaces;

public interface ISubmissionsService
{
    Task<bool> SetFeedbackAsync(SetFeedbackCommand setFeedbackCommand);
    Task<List<SubmissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<SubmissionDto>?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
}
