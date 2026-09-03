using System.Security.Claims;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Submissions;
using LMS.Api.Enums.Model;
using LMS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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

    // Puts a NameIdentifier claim on the controller's User, like the auth middleware would.
    private void SetUser(Guid userId)
    {
        ClaimsIdentity identity = new([new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
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
        SetUser(Guid.NewGuid());

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
        SetUser(Guid.NewGuid());

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

    [Fact]
    public async Task GetMe_WithAuthenticatedStudent_ShouldReturnOkWithSubmissions()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        List<SubmissionDto> submissions = [CreateDto(Guid.NewGuid())];

        _submissionsServiceMock
            .Setup(service => service.GetByStudentIdAsync(studentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submissions);

        ActionResult<List<SubmissionDto>> response = await _controller.GetMe(CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(submissions, result.Value);
    }

    [Fact]
    public async Task GetMe_WithoutUserIdClaim_ShouldReturnUnauthorized()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
        };

        ActionResult<List<SubmissionDto>> response = await _controller.GetMe(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(response.Result);
    }

    [Fact]
    public async Task CreateSubmission_WithValidData_ShouldReturnNoContent()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        SubmissionCreateDto dto = new() { ActivityId = Guid.NewGuid(), Text = "Assignment handed in." };

        _submissionsServiceMock
            .Setup(service => service.CreateSubmission(
                It.Is<SubmissionsCreateCommand>(c => c.StudentId == studentId && c.ActivityId == dto.ActivityId && c.Text == dto.Text),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        ActionResult<List<SubmissionDto>> response = await _controller.CreateSubmission(dto, CancellationToken.None);

        Assert.IsType<NoContentResult>(response.Result);
    }

    [Fact]
    public async Task CreateSubmission_WhenServiceFails_ShouldReturnNotFound()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        SubmissionCreateDto dto = new() { ActivityId = Guid.NewGuid(), Text = "Assignment handed in." };

        _submissionsServiceMock
            .Setup(service => service.CreateSubmission(It.IsAny<SubmissionsCreateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        ActionResult<List<SubmissionDto>> response = await _controller.CreateSubmission(dto, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task SetFeedback_WithValidData_ShouldReturnNoContent()
    {
        Guid teacherId = Guid.NewGuid();
        SetUser(teacherId);

        Guid submissionId = Guid.NewGuid();
        SubmissionFeedbackDto feedbackDto = new() { Feedback = "Good work." };

        _submissionsServiceMock
            .Setup(service => service.SetFeedbackAsync(
                It.Is<SetFeedbackCommand>(c => c.SubmissionId == submissionId && c.TeacherId == teacherId && c.Details == feedbackDto),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        ActionResult response = await _controller.SetFeedback(submissionId, feedbackDto, CancellationToken.None);

        Assert.IsType<NoContentResult>(response);
    }

    [Fact]
    public async Task SetFeedback_WhenServiceFails_ShouldReturnNotFound()
    {
        Guid teacherId = Guid.NewGuid();
        SetUser(teacherId);

        Guid submissionId = Guid.NewGuid();
        SubmissionFeedbackDto feedbackDto = new() { Feedback = "Good work." };

        _submissionsServiceMock
            .Setup(service => service.SetFeedbackAsync(It.IsAny<SetFeedbackCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        ActionResult response = await _controller.SetFeedback(submissionId, feedbackDto, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response);
    }
}
