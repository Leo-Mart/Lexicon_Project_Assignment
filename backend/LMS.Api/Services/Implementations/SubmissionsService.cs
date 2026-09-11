using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Module;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
using LMS.Api.Exceptions;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Interfaces;

namespace LMS.Api.Services.Implementations;

public class SubmissionsService(
        ISubmissionsRepository _submissionsRepository,
        IActivityService _activityService,
        IModuleService _moduleService,
        IEnrollmentService _enrollmentService,
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

    public async Task<PagedResponse<SubmissionDto>> GetPagedAsync(QueryParametersDto query, SubmissionReviewStatus? reviewStatus = null, CancellationToken cancellationToken = default)
    {
        PagedResponse<Submission> paged = await _submissionsRepository.GetPagedAsync(query, reviewStatus, cancellationToken);

        return new PagedResponse<SubmissionDto>
        {
            Items = _mapper.Map<List<SubmissionDto>>(paged.Items),
            TotalCount = paged.TotalCount,
            Page = paged.Page,
            PageSize = paged.PageSize
        };
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

    // Enrolled students with no submission, past the deadline - not a
    // Submission, so not a SubmissionDto. No deadline, or not passed yet,
    // means nobody can be overdue.
    public async Task<List<OverdueSubmissionDto>> GetOverdueByActivityIdAsync(
        Guid activityId,
        CancellationToken cancellationToken = default)
    {
        ActivityDto? activity = await _activityService.GetByIdAsync(activityId, cancellationToken);
        bool isPastDeadline = activity?.Deadline is not null && DateTime.UtcNow > activity.Deadline;

        if (activity is null || !isPastDeadline)
        {
            return [];
        }

        ModuleDto? module = await _moduleService.GetModuleById(activity.ModuleId);

        if (module is null)
        {
            return [];
        }

        List<Enrollment> enrollments =
            await _enrollmentService.GetEnrollmentsByCourseIdAsync(module.CourseId, cancellationToken);

        List<Submission> submissionsList =
            await _submissionsRepository.GetByActivityIdAsync(activityId, cancellationToken);
        HashSet<Guid> submittedStudentIds = submissionsList.Select(s => s.StudentId).ToHashSet();

        return enrollments
            .Where(enrollment => !submittedStudentIds.Contains(enrollment.StudentId))
            .Select(enrollment => new OverdueSubmissionDto
            {
                ActivityId = activityId,
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student.Name,
            })
            .ToList();
    }

    public async Task<SubmissionDto> CreateSubmission(SubmissionsCreateCommand command, CancellationToken cancellationToken)
    {
        DateTime submittedAt = DateTime.UtcNow;
        ActivityDto? activity = await _activityService.GetByIdAsync(command.ActivityId, cancellationToken);

        // Only hand-in work types take a submission - not lectures/e-learning/other.
        if (activity is not null
            && activity.Type != ActivityType.Task
            && activity.Type != ActivityType.Practice)
        {
            throw new InvalidActivityTypeException(
                $"Activities of type {activity.Type} cannot take submissions.",
                400);
        }

        // No deadline (or no activity found) means it can't be late.
        bool isLate = activity?.Deadline is not null && submittedAt > activity.Deadline;

        Submission submission = new()
        {
            ActivityId = command.ActivityId,
            StudentId = command.StudentId,
            Text = command.Text,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SubmittedAt = submittedAt,
        };

        await _submissionsRepository.CreateAsync(submission, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // The submission's own Activity nav isn't loaded here, so set SubmittedLate directly.
        SubmissionDto dto = _mapper.Map<SubmissionDto>(submission);
        dto.SubmittedLate = isLate;
        return dto;
    }

    public async Task<SubmissionDto?> UpdateSubmission(SubmissionsUpdateCommand command, CancellationToken cancellationToken)
    {
        Submission? submission = await _submissionsRepository.GetByIdAsync(command.SubmissionId, cancellationToken);
        if (submission is null)
        {
            return null;
        }

        // Only resubmittable while a teacher has flagged it as needing completion.
        if (submission.ReviewStatus != SubmissionReviewStatus.NeedsCompletion)
        {
            throw new InvalidSubmissionStateException(
                "Only a submission needing completion can be resubmitted.",
                400);
        }

        submission.Text = command.Text;
        submission.SubmittedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;
        submission.ResubmittedAt = DateTime.UtcNow;

        // A fresh submission clears the old review - the teacher hasn't seen this version yet.
        submission.ReviewStatus = null;
        submission.Feedback = null;
        submission.FeedbackByTeacherId = null;
        submission.FeedbackAt = null;

        _submissionsRepository.Update(submission);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubmissionDto>(submission);
    }
}
