using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Activities;
using LMS.Api.DTOs.Common;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
using LMS.Api.Exceptions;
using LMS.Api.Mappings;
using LMS.Api.Models;
using LMS.Api.Repositories.Interfaces;
using LMS.Api.Services.Implementations;
using LMS.Api.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace LMS.Api.Tests.Services.Submissions;

public class SubmissionsServiceTests
{
    private readonly Mock<ISubmissionsRepository> _submissionsRepositoryMock;
    private readonly Mock<IActivityService> _activityServiceMock;
    private readonly Mock<IModuleService> _moduleServiceMock;
    private readonly Mock<IEnrollmentService> _enrollmentServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ISubmissionsService _submissionsService;

    public SubmissionsServiceTests()
    {
        _submissionsRepositoryMock = new Mock<ISubmissionsRepository>();
        _activityServiceMock = new Mock<IActivityService>();
        _moduleServiceMock = new Mock<IModuleService>();
        _enrollmentServiceMock = new Mock<IEnrollmentService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // A real mapper, not a mock: the service's job is to map, so a
        // stubbed IMapper would leave these assertions testing nothing.
        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<SubmissionsProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();

        _submissionsService = new SubmissionsService(
            _submissionsRepositoryMock.Object,
            _activityServiceMock.Object,
            _moduleServiceMock.Object,
            _enrollmentServiceMock.Object,
            _unitOfWorkMock.Object,
            mapper
        );
    }

    private static Submission CreateSubmission(Guid submissionId)
    {
        return new Submission
        {
            SubmissionId = submissionId,
            ActivityId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Text = "Assignment handed in.",
            SubmittedAt = new DateTime(2026, 10, 1, 14, 30, 0, DateTimeKind.Utc),
            Feedback = "Good work.",
            FeedbackByTeacherId = Guid.NewGuid(),
            FeedbackAt = new DateTime(2026, 10, 3, 10, 0, 0, DateTimeKind.Utc),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetAllAsync_WithSubmissions_ShouldReturnMappedSubmissions()
    {
        List<Submission> submissions =
        [
            CreateSubmission(Guid.NewGuid()),
            CreateSubmission(Guid.NewGuid())
        ];

        _submissionsRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(submissions);

        List<SubmissionDto> result = await _submissionsService.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(submissions[0].SubmissionId, result[0].SubmissionId);
        Assert.Equal(submissions[0].ActivityId, result[0].ActivityId);
        Assert.Equal(submissions[0].StudentId, result[0].StudentId);
        Assert.Equal(submissions[0].Text, result[0].Text);

        _submissionsRepositoryMock.Verify(
            repository => repository.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAllAsync_WithNoSubmissions_ShouldReturnEmptyList()
    {
        _submissionsRepositoryMock
            .Setup(repository => repository.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        List<SubmissionDto> result = await _submissionsService.GetAllAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPagedAsync_WithSubmissions_ShouldReturnMappedPage()
    {
        List<Submission> submissions =
        [
            CreateSubmission(Guid.NewGuid()),
            CreateSubmission(Guid.NewGuid())
        ];
        QueryParametersDto query = new() { Page = 1, PageSize = 10 };

        _submissionsRepositoryMock
            .Setup(repository => repository.GetPagedAsync(query, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResponse<Submission>
            {
                Items = submissions,
                TotalCount = 2,
                Page = 1,
                PageSize = 10
            });

        PagedResponse<SubmissionDto> result = await _submissionsService.GetPagedAsync(query);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(submissions[0].SubmissionId, result.Items[0].SubmissionId);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingSubmission_ShouldReturnMappedSubmission()
    {
        Guid submissionId = Guid.NewGuid();
        Submission submission = CreateSubmission(submissionId);

        _submissionsRepositoryMock
            .Setup(repository => repository.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        SubmissionDto? result = await _submissionsService.GetByIdAsync(submissionId);

        Assert.NotNull(result);
        Assert.Equal(submissionId, result.SubmissionId);
        Assert.Equal(submission.Feedback, result.Feedback);
        Assert.Equal(submission.FeedbackByTeacherId, result.FeedbackByTeacherId);
        Assert.Equal(submission.FeedbackAt, result.FeedbackAt);
    }

    [Fact]
    public async Task GetByIdAsync_WithSubmissionAfterDeadline_ShouldBeLate()
    {
        Guid submissionId = Guid.NewGuid();
        Submission submission = CreateSubmission(submissionId);
        submission.Activity = new Activity { Deadline = submission.SubmittedAt.AddDays(-1) };

        _submissionsRepositoryMock
            .Setup(repository => repository.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        SubmissionDto? result = await _submissionsService.GetByIdAsync(submissionId);

        Assert.NotNull(result);
        Assert.True(result.SubmittedLate);
    }

    [Fact]
    public async Task GetByIdAsync_WithMissingSubmission_ShouldReturnNull()
    {
        Guid submissionId = Guid.NewGuid();

        _submissionsRepositoryMock
            .Setup(repository => repository.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Submission?)null);

        SubmissionDto? result = await _submissionsService.GetByIdAsync(submissionId);

        Assert.Null(result);
    }

    private static SubmissionsCreateCommand CreateCommand(Guid activityId)
    {
        return new SubmissionsCreateCommand
        {
            StudentId = Guid.NewGuid(),
            ActivityId = activityId,
            Text = "Assignment handed in."
        };
    }

    [Fact]
    public async Task CreateSubmission_BeforeDeadline_ShouldNotBeLate()
    {
        Guid activityId = Guid.NewGuid();
        SubmissionsCreateCommand command = CreateCommand(activityId);

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ActivityDto { ActivityId = activityId, Type = ActivityType.Task, Deadline = DateTime.UtcNow.AddDays(1) });

        SubmissionDto result = await _submissionsService.CreateSubmission(command, CancellationToken.None);

        Assert.False(result.SubmittedLate);
    }

    [Fact]
    public async Task CreateSubmission_AfterDeadline_ShouldBeLate()
    {
        Guid activityId = Guid.NewGuid();
        SubmissionsCreateCommand command = CreateCommand(activityId);

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ActivityDto { ActivityId = activityId, Type = ActivityType.Task, Deadline = DateTime.UtcNow.AddDays(-1) });

        SubmissionDto result = await _submissionsService.CreateSubmission(command, CancellationToken.None);

        Assert.True(result.SubmittedLate);
    }

    [Theory]
    [InlineData(ActivityType.Lecture)]
    [InlineData(ActivityType.ELearning)]
    [InlineData(ActivityType.Other)]
    public async Task CreateSubmission_ForNonSubmittableType_ShouldThrow(ActivityType type)
    {
        Guid activityId = Guid.NewGuid();
        SubmissionsCreateCommand command = CreateCommand(activityId);

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ActivityDto { ActivityId = activityId, Type = type });

        await Assert.ThrowsAsync<InvalidActivityTypeException>(
            () => _submissionsService.CreateSubmission(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateSubmission_ShouldAgreeWithMappingProfile_WhenActivityIsLoaded()
    {
        // CreateSubmission sets SubmittedLate manually since the new submission's
        // Activity nav isn't loaded; this checks that value against what the
        // mapping profile would compute once the nav is loaded (e.g. via GetByIdAsync).
        Guid activityId = Guid.NewGuid();
        SubmissionsCreateCommand command = CreateCommand(activityId);
        DateTime deadline = DateTime.UtcNow.AddSeconds(-1);

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ActivityDto { ActivityId = activityId, Type = ActivityType.Task, Deadline = deadline });

        Submission? createdSubmission = null;
        _submissionsRepositoryMock
            .Setup(repository => repository.CreateAsync(It.IsAny<Submission>(), It.IsAny<CancellationToken>()))
            .Callback<Submission, CancellationToken>((submission, _) => createdSubmission = submission)
            .Returns(Task.CompletedTask);

        SubmissionDto result = await _submissionsService.CreateSubmission(command, CancellationToken.None);

        createdSubmission!.Activity = new Activity { Deadline = deadline };
        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<SubmissionsProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();
        SubmissionDto mapped = mapper.Map<SubmissionDto>(createdSubmission);

        // The deadline is already past, so both paths must land on true, not
        // just agree with each other (which they'd also do if both were wrong).
        Assert.True(result.SubmittedLate);
        Assert.Equal(mapped.SubmittedLate, result.SubmittedLate);
    }

    [Fact]
    public async Task SetFeedbackAsync_WithExistingSubmission_ShouldUpdateFeedbackAndReviewStatus()
    {
        Guid submissionId = Guid.NewGuid();
        Submission submission = CreateSubmission(submissionId);

        _submissionsRepositoryMock
            .Setup(repository => repository.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        SetFeedbackCommand command = new()
        {
            SubmissionId = submissionId,
            TeacherId = Guid.NewGuid(),
            Details = new SubmissionFeedbackDto { Feedback = "Needs more tests.", ReviewStatus = SubmissionReviewStatus.NeedsCompletion }
        };

        SubmissionDto? result = await _submissionsService.SetFeedbackAsync(command);

        Assert.NotNull(result);
        Assert.Equal(SubmissionReviewStatus.NeedsCompletion, result.ReviewStatus);
        Assert.Equal("Needs more tests.", result.Feedback);
    }

    [Fact]
    public async Task CreateSubmission_WithNoDeadline_ShouldNotBeLate()
    {
        Guid activityId = Guid.NewGuid();
        SubmissionsCreateCommand command = CreateCommand(activityId);

        _activityServiceMock
            .Setup(service => service.GetByIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ActivityDto { ActivityId = activityId, Type = ActivityType.Task, Deadline = null });

        SubmissionDto result = await _submissionsService.CreateSubmission(command, CancellationToken.None);

        Assert.False(result.SubmittedLate);
    }
}
