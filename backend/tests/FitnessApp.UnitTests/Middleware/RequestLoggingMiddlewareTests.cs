using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using FitnessApp.API.Middleware;

namespace FitnessApp.UnitTests.Middleware;

public class RequestLoggingMiddlewareTests
{
    private readonly Mock<ILogger<RequestLoggingMiddleware>> _loggerMock;

    public RequestLoggingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<RequestLoggingMiddleware>>();
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "GET";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogRequestStart()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "GET";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("started")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogRequestCompletion()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "POST";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("completed")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogElapsedTime()
    {
        // Arrange
        RequestDelegate next = async (HttpContext ctx) =>
        {
            await Task.Delay(10); // Simulate some work
        };

        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "GET";

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("ms")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_EvenWhenExceptionThrown_ShouldLogCompletion()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => throw new Exception("Test exception");
        var middleware = new RequestLoggingMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/test";
        context.Request.Method = "GET";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => middleware.InvokeAsync(context));

        // Verify completion was still logged (in finally block)
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("completed")),
                null,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}
