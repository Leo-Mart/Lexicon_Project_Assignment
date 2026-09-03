using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations;

public class SubmissionsService(
        ISubmissionsRepository _submissionsRepository,
        IUnitOfWork _unitOfWork,
        IMapper _mapper) : ISubmissionsService
{
    //Enum exists
    //    public enum SubmissionStatus
    // {
    //     Submitted = 1,
    //     Late = 2
    // }

    public async Task<bool> SetFeedbackAsync(SetFeedbackCommand setFeedbackCommand, CancellationToken cancellationToken = default)
    {
        Submission? submission = await _submissionsRepository.GetByIdAsync(setFeedbackCommand.SubmissionId, cancellationToken);
        if (submission == null)
        {
            return false;
        }
        submission.Feedback = setFeedbackCommand.Details.Feedback;
        submission.FeedbackByTeacherId = setFeedbackCommand.TeacherId;
        submission.FeedbackAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;
        _submissionsRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;

    }

    public async Task<List<SubmissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Submission> resources = await _submissionsRepository.GetAllAsync(cancellationToken);

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

    public async Task<List<SubmissionDto>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        List<Submission> submissionsList =
          await _submissionsRepository.GetByStudentIdAsync(studentId, cancellationToken);

        return _mapper.Map<List<SubmissionDto>>(submissionsList);
    }

    public async Task<bool> CreateSubmission(SubmissionsCreateCommand command, CancellationToken cancellationToken)
    {
        Submission submission = new()
        {
            ActivityId = command.ActivityId,
            StudentId = command.StudentId,
            Text = command.Text,
            CreatedAt = DateTime.UtcNow,
            SubmittedAt = DateTime.UtcNow,
            Status = SubmissionStatus.Submitted,
        };

        await _submissionsRepository.CreateAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
