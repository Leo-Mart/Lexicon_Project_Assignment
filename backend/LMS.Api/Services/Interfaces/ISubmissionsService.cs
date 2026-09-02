
using LMS.Api.DTOs.Submissions;

namespace LMS.Api.Services.Interfaces;

public interface ISubmissionsService
{
    Task<List<SubmissionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SubmissionDto?> GetByIdAsync(Guid resourceId, CancellationToken cancellationToken = default);
}
