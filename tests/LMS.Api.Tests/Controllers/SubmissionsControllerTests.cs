using System.Security.Claims;
using LMS.Api.Constants;
using LMS.Api.Controllers;
using LMS.Api.DTOs.Common;
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

    // Puts a NameIdentifier claim (and optionally a role claim) on the
    // controller's User, like the auth middleware would.
    private void SetUser(Guid userId, string? role = null)
    {
        List<Claim> claims = [new Claim(ClaimTypes.NameIdentifier, userId.ToString())];
        if (role is not null)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        ClaimsIdentity identity = new(claims, "TestAuth");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) }
        };
    }

    // No NameIdentifier claim at all, like an unauthenticated request.
    private void SetUserWithoutClaim()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
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
    public async Task GetPaged_WithSubmissions_ShouldReturnOkWithPage()
    {
        QueryParametersDto query = new() { Page = 1, PageSize = 10 };
        PagedResponse<SubmissionDto> page = new()
        {
            Items = [CreateDto(Guid.NewGuid()), CreateDto(Guid.NewGuid())],
            TotalCount = 2,
            Page = 1,
            PageSize = 10
        };

        _submissionsServiceMock
            .Setup(service => service.GetPagedAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(page);

        ActionResult<PagedResponse<SubmissionDto>> response =
            await _controller.GetPaged(query, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        PagedResponse<SubmissionDto> value = Assert.IsType<PagedResponse<SubmissionDto>>(result.Value);

        Assert.Same(page, value);
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
    public async Task GetById_WithoutUserIdClaim_ShouldReturnUnauthorized()
    {
        SetUserWithoutClaim();

        ActionResult<SubmissionDto> response = await _controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(response.Result);
    }

    [Fact]
    public async Task GetById_WhenStudentAccessesOwnSubmission_ShouldReturnOk()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId, RoleConstants.Student);

        Guid submissionId = Guid.NewGuid();
        SubmissionDto submission = CreateDto(submissionId);
        submission.StudentId = studentId;

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        ActionResult<SubmissionDto> response = await _controller.GetById(submissionId, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(submission, result.Value);
    }

    [Fact]
    public async Task GetById_WhenStudentAccessesOthersSubmission_ShouldReturnForbid()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId, RoleConstants.Student);

        Guid submissionId = Guid.NewGuid();
        SubmissionDto submission = CreateDto(submissionId);

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        ActionResult<SubmissionDto> response = await _controller.GetById(submissionId, CancellationToken.None);

        Assert.IsType<ForbidResult>(response.Result);
    }

    [Fact]
    public async Task GetById_WhenTeacherAccessesOthersSubmission_ShouldReturnOk()
    {
        Guid teacherId = Guid.NewGuid();
        SetUser(teacherId, RoleConstants.Teacher);

        Guid submissionId = Guid.NewGuid();
        SubmissionDto submission = CreateDto(submissionId);

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submission);

        ActionResult<SubmissionDto> response = await _controller.GetById(submissionId, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(submission, result.Value);
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
        SetUserWithoutClaim();

        ActionResult<List<SubmissionDto>> response = await _controller.GetMe(CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(response.Result);
    }

    [Fact]
    public async Task CreateSubmission_WithValidData_ShouldReturnOkWithSubmission()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        SubmissionCreateDto dto = new() { ActivityId = Guid.NewGuid(), Text = "Assignment handed in." };
        SubmissionDto created = CreateDto(Guid.NewGuid());

        _submissionsServiceMock
            .Setup(service => service.CreateSubmission(
                It.Is<SubmissionsCreateCommand>(c => c.StudentId == studentId && c.ActivityId == dto.ActivityId && c.Text == dto.Text),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        ActionResult<SubmissionDto> response = await _controller.CreateSubmission(dto, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(created, result.Value);
    }

    [Fact]
    public async Task CreateSubmission_WithoutUserIdClaim_ShouldReturnUnauthorized()
    {
        SetUserWithoutClaim();

        SubmissionCreateDto dto = new() { ActivityId = Guid.NewGuid(), Text = "Assignment handed in." };

        ActionResult<SubmissionDto> response = await _controller.CreateSubmission(dto, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(response.Result);
    }

    [Fact]
    public async Task UpdateSubmission_WithOwnSubmission_ShouldReturnOkWithSubmission()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        Guid submissionId = Guid.NewGuid();
        SubmissionDto existing = CreateDto(submissionId);
        existing.StudentId = studentId;
        SubmissionUpdateDto dto = new() { Text = "Revised submission." };
        SubmissionDto updated = CreateDto(submissionId);

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _submissionsServiceMock
            .Setup(service => service.UpdateSubmission(
                It.Is<SubmissionsUpdateCommand>(c => c.SubmissionId == submissionId && c.StudentId == studentId && c.Text == dto.Text),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        ActionResult<SubmissionDto> response = await _controller.UpdateSubmission(submissionId, dto, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(updated, result.Value);
    }

    [Fact]
    public async Task UpdateSubmission_WithMissingSubmission_ShouldReturnNotFound()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        Guid submissionId = Guid.NewGuid();
        SubmissionUpdateDto dto = new() { Text = "Revised submission." };

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubmissionDto?)null);

        ActionResult<SubmissionDto> response = await _controller.UpdateSubmission(submissionId, dto, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response.Result);

        _submissionsServiceMock.Verify(
            service => service.UpdateSubmission(It.IsAny<SubmissionsUpdateCommand>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task UpdateSubmission_WithDifferentStudent_ShouldReturnForbid()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        Guid submissionId = Guid.NewGuid();
        SubmissionDto existing = CreateDto(submissionId);
        SubmissionUpdateDto dto = new() { Text = "Revised submission." };

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        ActionResult<SubmissionDto> response = await _controller.UpdateSubmission(submissionId, dto, CancellationToken.None);

        Assert.IsType<ForbidResult>(response.Result);
    }

    [Fact]
    public async Task UpdateSubmission_WhenServiceReturnsNull_ShouldReturnNotFound()
    {
        Guid studentId = Guid.NewGuid();
        SetUser(studentId);

        Guid submissionId = Guid.NewGuid();
        SubmissionDto existing = CreateDto(submissionId);
        existing.StudentId = studentId;
        SubmissionUpdateDto dto = new() { Text = "Revised submission." };

        _submissionsServiceMock
            .Setup(service => service.GetByIdAsync(submissionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _submissionsServiceMock
            .Setup(service => service.UpdateSubmission(It.IsAny<SubmissionsUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubmissionDto?)null);

        ActionResult<SubmissionDto> response = await _controller.UpdateSubmission(submissionId, dto, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task UpdateSubmission_WithoutUserIdClaim_ShouldReturnUnauthorized()
    {
        SetUserWithoutClaim();

        SubmissionUpdateDto dto = new() { Text = "Revised submission." };

        ActionResult<SubmissionDto> response = await _controller.UpdateSubmission(Guid.NewGuid(), dto, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(response.Result);
    }

    [Fact]
    public async Task SetFeedback_WithValidData_ShouldReturnOkWithSubmission()
    {
        Guid teacherId = Guid.NewGuid();
        SetUser(teacherId);

        Guid submissionId = Guid.NewGuid();
        SubmissionFeedbackDto feedbackDto = new() { Feedback = "Good work.", ReviewStatus = SubmissionReviewStatus.Approved };
        SubmissionDto updated = CreateDto(submissionId);

        _submissionsServiceMock
            .Setup(service => service.SetFeedbackAsync(
                It.Is<SetFeedbackCommand>(c => c.SubmissionId == submissionId && c.TeacherId == teacherId && c.Details == feedbackDto),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        ActionResult response = await _controller.SetFeedback(submissionId, feedbackDto, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response);
        Assert.Same(updated, result.Value);
    }

    [Fact]
    public async Task SetFeedback_WhenServiceFails_ShouldReturnNotFound()
    {
        Guid teacherId = Guid.NewGuid();
        SetUser(teacherId);

        Guid submissionId = Guid.NewGuid();
        SubmissionFeedbackDto feedbackDto = new() { Feedback = "Good work.", ReviewStatus = SubmissionReviewStatus.Approved };

        _submissionsServiceMock
            .Setup(service => service.SetFeedbackAsync(It.IsAny<SetFeedbackCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubmissionDto?)null);

        ActionResult response = await _controller.SetFeedback(submissionId, feedbackDto, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response);
    }

    [Fact]
    public async Task SetFeedback_WithoutUserIdClaim_ShouldReturnUnauthorized()
    {
        SetUserWithoutClaim();

        SubmissionFeedbackDto feedbackDto = new() { Feedback = "Good work.", ReviewStatus = SubmissionReviewStatus.Approved };

        ActionResult response = await _controller.SetFeedback(Guid.NewGuid(), feedbackDto, CancellationToken.None);

        Assert.IsType<UnauthorizedResult>(response);
    }

    [Fact]
    public async Task GetByActivityIdAsync_WhenStudent_ShouldReturnForbid()
    {
        SetUser(Guid.NewGuid(), RoleConstants.Student);

        ActionResult<List<SubmissionDto>> response =
            await _controller.GetByActivityIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<ForbidResult>(response.Result);
    }

    [Fact]
    public async Task GetByActivityIdAsync_WithSubmissions_ShouldReturnOkWithSubmissions()
    {
        SetUser(Guid.NewGuid(), RoleConstants.Teacher);

        Guid activityId = Guid.NewGuid();
        List<SubmissionDto> submissions = [CreateDto(Guid.NewGuid())];

        _submissionsServiceMock
            .Setup(service => service.GetByActivityIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(submissions);

        ActionResult<List<SubmissionDto>> response =
            await _controller.GetByActivityIdAsync(activityId, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(submissions, result.Value);
    }

    [Fact]
    public async Task GetByActivityIdAsync_WithNoSubmissions_ShouldReturnNotFound()
    {
        SetUser(Guid.NewGuid(), RoleConstants.Teacher);

        Guid activityId = Guid.NewGuid();

        _submissionsServiceMock
            .Setup(service => service.GetByActivityIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        ActionResult<List<SubmissionDto>> response =
            await _controller.GetByActivityIdAsync(activityId, CancellationToken.None);

        Assert.IsType<NotFoundResult>(response.Result);
    }

    [Fact]
    public async Task GetOverdueByActivityIdAsync_ShouldReturnOkWithOverdueList()
    {
        Guid activityId = Guid.NewGuid();
        List<OverdueSubmissionDto> overdue =
            [new OverdueSubmissionDto { ActivityId = activityId, StudentId = Guid.NewGuid(), StudentName = "Overdue Student" }];

        _submissionsServiceMock
            .Setup(service => service.GetOverdueByActivityIdAsync(activityId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(overdue);

        ActionResult<List<OverdueSubmissionDto>> response =
            await _controller.GetOverdueByActivityIdAsync(activityId, CancellationToken.None);

        OkObjectResult result = Assert.IsType<OkObjectResult>(response.Result);
        Assert.Same(overdue, result.Value);
    }
}
