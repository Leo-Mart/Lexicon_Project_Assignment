using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations;

public class SubmissionsService(
        ISubmissionsRepository _submissionsRepository,
        IUnitOfWork _unitOfWork,
        IMapper _mapper) : ISubmissionsService
{
    public void SetFeedbackAsync(Guid activityId, Guid studentId, string feedbackText)
    {
        throw new NotImplementedException();
    }

    public async Task<List<SubmissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Submission> resources = await _submissionsRepository.GetAllAsync(cancellationToken);

        //HACK
        Console.WriteLine(_unitOfWork + "This is a hack, will be removed later");
        return _mapper.Map<List<SubmissionDto>>(resources);
    }

    public async Task<SubmissionDto?> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken = default)
    {
        Submission? submission =
            await _submissionsRepository.GetByIdAsync(
                submissionId,
                cancellationToken
            );

        return submission is null
            ? null
            : _mapper.Map<SubmissionDto>(submission);
    }

    public async Task<List<SubmissionDto>?> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        List<Submission> submissionsList =
          await _submissionsRepository.GetByStudentIdAsync(studentId, cancellationToken);

        return submissionsList.Count == 0
       ? null
       : _mapper.Map<List<SubmissionDto>>(submissionsList);
    }

    Task<bool> ISubmissionsService.SetFeedbackAsync(Guid activityId, Guid studentId, string feedbackText)
    {
        throw new NotImplementedException();
    }
}
