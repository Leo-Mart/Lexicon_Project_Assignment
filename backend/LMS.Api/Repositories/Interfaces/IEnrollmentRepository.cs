using LMS.Api.Models;

namespace LMS.Api.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<Enrollment?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task<List<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task AddAsync(Enrollment enrollment, CancellationToken cancellationToken = default);

    void Update(Enrollment enrollment);
    void Delete(Enrollment enrollment);
}
