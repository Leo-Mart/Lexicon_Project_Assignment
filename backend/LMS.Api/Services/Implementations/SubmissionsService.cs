using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations;

public class SubmissionsService(
        ISubmissionsRepository _submissionsRepository,
        IActivityService _activityService,
        IUnitOfWork _unitOfWork,
        IMapper _mapper) : ISubmissionsService
{

    public async Task<SubmissionDto?> SetFeedbackAsync(SetFeedbackCommand setFeedbackCommand, CancellationToken cancellationToken = default)
    {
        Submission? submission = await _submissionsRepository.GetByIdAsync(setFeedbackCommand.SubmissionId, cancellationToken);
        if (submission == null)
        {
            return null;
        }
        submission.Feedback = setFeedbackCommand.Details.Feedback;
        submission.ReviewStatus = setFeedbackCommand.Details.ReviewStatus;
        submission.FeedbackByTeacherId = setFeedbackCommand.TeacherId;
        submission.FeedbackAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;
        _submissionsRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubmissionDto>(submission);
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

    public async Task<List<SubmissionDto>> GetByActivityIdAsync(Guid activityId, CancellationToken cancellationToken = default)
    {
        List<Submission> submissionsList =
          await _submissionsRepository.GetByActivityIdAsync(activityId, cancellationToken);

        return _mapper.Map<List<SubmissionDto>>(submissionsList);
    }

    public async Task<SubmissionDto> CreateSubmission(SubmissionsCreateCommand command, CancellationToken cancellationToken)
    {
        DateTime submittedAt = DateTime.UtcNow;
        ActivityDto? activity = await _activityService.GetByIdAsync(command.ActivityId, cancellationToken);

        // No deadline (or no activity found) means it can't be late.
        bool isLate = activity?.Deadline is not null && submittedAt > activity.Deadline;

        Submission submission = new()
        {
            ActivityId = command.ActivityId,
            StudentId = command.StudentId,
            Text = command.Text,
            CreatedAt = DateTime.UtcNow,
            SubmittedAt = submittedAt,
        };

        await _submissionsRepository.CreateAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The submission's own Activity nav isn't loaded here, so set IsLate directly.
        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);
        dto.IsLate = isLate;
        return dto;
    }
}
