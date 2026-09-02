using System.Text.Json;
using LMS.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace LMS.Api.Tests.Exceptions;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
    private readonly GlobalExceptionHandler _exceptionHandler;

    public GlobalExceptionHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
        _exceptionHandler = new GlobalExceptionHandler(_loggerMock.Object);
    }

    [Fact]
    public async Task TryHandleAsync_WithInvalidDateException_ShouldReturnBadRequest()
    {
        DefaultHttpContext httpContext = CreateHttpContext();

        InvalidDateException exception = new(
            "Start date cannot be in the past.",
            StatusCodes.Status400BadRequest
        );

        bool handled = await _exceptionHandler.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None
        );

        ProblemDetails problemDetails = await ReadProblemDetailsAsync(httpContext);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Invalid date", problemDetails.Title);
        Assert.Equal(exception.Message, problemDetails.Detail);
        Assert.Equal("/api/activity", problemDetails.Instance);
    }

    [Fact]
    public async Task TryHandleAsync_WithOverlappingDateException_ShouldReturnBadRequest()
    {
        DefaultHttpContext httpContext = CreateHttpContext();

        OverlappingDateException exception = new(
            "Activity overlaps with another activity.",
            StatusCodes.Status400BadRequest
        );

        bool handled = await _exceptionHandler.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None
        );

        ProblemDetails problemDetails = await ReadProblemDetailsAsync(httpContext);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, problemDetails.Status);
        Assert.Equal("Overlapping dates", problemDetails.Title);
        Assert.Equal(exception.Message, problemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithKeyNotFoundException_ShouldReturnNotFound()
    {
        DefaultHttpContext httpContext = CreateHttpContext();

        KeyNotFoundException exception = new("Module not found.");

        bool handled = await _exceptionHandler.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None
        );

        ProblemDetails problemDetails = await ReadProblemDetailsAsync(httpContext);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, httpContext.Response.StatusCode);
        Assert.Equal(StatusCodes.Status404NotFound, problemDetails.Status);
        Assert.Equal("Resource not found", problemDetails.Title);
        Assert.Equal(exception.Message, problemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithUnknownException_ShouldReturnInternalServerError()
    {
        DefaultHttpContext httpContext = CreateHttpContext();

        InvalidOperationException exception = new("Unexpected error.");

        bool handled = await _exceptionHandler.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None
        );

        ProblemDetails problemDetails = await ReadProblemDetailsAsync(httpContext);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemDetails.Status);
        Assert.Equal("Internal server error", problemDetails.Title);
        Assert.Equal(exception.Message, problemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_WithUnknownException_ShouldLogError()
    {
        DefaultHttpContext httpContext = CreateHttpContext();

        InvalidOperationException exception = new("Unexpected error.");

        await _exceptionHandler.TryHandleAsync(
            httpContext,
            exception,
            CancellationToken.None
        );

        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once
        );
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        DefaultHttpContext httpContext = new();

        httpContext.Request.Path = "/api/activity";
        httpContext.Response.Body = new MemoryStream();

        return httpContext;
    }

    private static async Task<ProblemDetails> ReadProblemDetailsAsync(DefaultHttpContext httpContext)
    {
        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        ProblemDetails? problemDetails =
            await JsonSerializer.DeserializeAsync<ProblemDetails>(
                httpContext.Response.Body,
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
            );

        return problemDetails
            ?? throw new InvalidOperationException(
                "Could not deserialize ProblemDetails response."
            );
    }
}
