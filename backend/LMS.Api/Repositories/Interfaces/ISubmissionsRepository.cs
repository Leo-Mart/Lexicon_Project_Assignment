using LMS.Api.DTOs.Submissions;
using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface ISubmissionsRepository
{
    Task CreateAsync(Submission submission, CancellationToken cancellationToken);
    Task<List<Submission>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Submission?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default);
    Task<List<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);
    public void Update(Submission submission);

}
