using AutoMapper;
using LMS.Api.Data.UnitOfWork;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
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
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ISubmissionsService _submissionsService;

    public SubmissionsServiceTests()
    {
        _submissionsRepositoryMock = new Mock<ISubmissionsRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // A real mapper, not a mock: the service's job is to map, so a
        // stubbed IMapper would leave these assertions testing nothing.
        IMapper mapper = new MapperConfiguration(
            cfg => cfg.AddProfile<SubmissionsProfile>(),
            NullLoggerFactory.Instance
        ).CreateMapper();

        _submissionsService = new SubmissionsService(
            _submissionsRepositoryMock.Object,
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
            Status = SubmissionStatus.Submitted,
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
        Assert.Equal(submissions[0].Status, result[0].Status);

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
    public async Task GetByIdAsync_WithMissingSubmission_ShouldReturnNull()
    {
        Guid submissionId = Guid.NewGuid();

        _submissionsRepositoryMock
            .Setup(repository => repository.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Submission?)null);

        SubmissionDto? result = await _submissionsService.GetByIdAsync(submissionId);

        Assert.Null(result);
    }
}
