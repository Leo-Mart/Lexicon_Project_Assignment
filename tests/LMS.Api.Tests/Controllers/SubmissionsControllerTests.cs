using LMS.Api.Controllers;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LMS.Api.Tests.Controllers;

public class SubmissionsControllerTests
{
    private readonly Mock<ISubmissionsService> _submissionsServiceMock;
    private readonly SubmissionsController _controller;

    public SubmissionsControllerTests()
    {
        _submissionsServiceMock = new Mock<ISubmissionsService>();
        _controller = new SubmissionsController(_submissionsServiceMock.Object);
    }

    private static SubmissionDto CreateDto(Guid submissionId)
    {
        return new SubmissionDto
        {
            SubmissionId = submissionId,
            ActivityId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Text = "Assignment handed in.",
            SubmittedAt = new DateTime(2026, 10, 1, 14, 30, 0, DateTimeKind.Utc),
            Status = SubmissionStatus.Submitted,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task GetAll_WithSubmissions_ShouldReturnOkWithSubmissions()
    {
        List<SubmissionDto> submissions =
        [
            CreateDto(Guid.NewGuid()),
            CreateDto(Guid.NewGuid())
        ];

        _submissionsServiceMock
            .Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(submissions);

        ActionResult<List<SubmissionDto>> response =
            await _controller.GetAll(CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        List<SubmissionDto> value = Assert.IsType<List<SubmissionDto>>(result.Value);

        Assert.Equal(2, value.Count);
        Assert.Same(submissions, value);
    }

    [Fact]
    public async Task GetAll_WithNoSubmissions_ShouldReturnOkWithEmptyList()
    {
        _submissionsServiceMock
            .Setup(service => service.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        ActionResult<List<SubmissionDto>> response =
            await _controller.GetAll(CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        List<SubmissionDto> value = Assert.IsType<List<SubmissionDto>>(result.Value);

        Assert.Empty(value);
    }

    [Fact]
    public async Task GetById_WithExistingSubmission_ShouldReturnOkWithSubmission()
    {
        Guid submissionId = Guid.NewGuid();
        SubmissionDto submission = CreateDto(submissionId);

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        ActionResult<SubmissionDto> response =
            await _controller.GetById(submissionId, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        SubmissionDto value = Assert.IsType<SubmissionDto>(result.Value);

        Assert.Equal(submissionId, value.SubmissionId);
    }

    [Fact]
    public async Task GetById_WithMissingSubmission_ShouldReturnNotFound()
    {
        Guid submissionId = Guid.NewGuid();

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubmissionDto?)null);

        ActionResult<SubmissionDto> response =
            await _controller.GetById(submissionId, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response.Result);

        _submissionsServiceMock.Verify(
            service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
