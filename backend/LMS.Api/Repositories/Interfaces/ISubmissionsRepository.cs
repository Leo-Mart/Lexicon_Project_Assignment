using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface ISubmissionsRepository
{
    Task<List<Submission>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<List<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

}
