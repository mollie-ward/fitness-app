using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
using FitnessApp.API.Middleware;

namespace FitnessApp.UnitTests.Middleware;

public class GlobalExceptionHandlerMiddlewareTests
{
    private readonly Mock<ILogger<GlobalExceptionHandlerMiddleware>> _loggerMock;
    private readonly Mock<IHostEnvironment> _hostEnvironmentMock;

    public GlobalExceptionHandlerMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<GlobalExceptionHandlerMiddleware>>();
        _hostEnvironmentMock = new Mock<IHostEnvironment>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
    {
        // Arrange
        var nextCalled = false;
        RequestDelegate next = (HttpContext ctx) =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        };

        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task InvokeAsync_WhenArgumentNullException_ShouldReturnBadRequest()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => throw new ArgumentNullException("test");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        _hostEnvironmentMock.Setup(e => e.EnvironmentName).Returns("Development");
        context.RequestServices = CreateServiceProvider(_hostEnvironmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(400);
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedException_ShouldReturnUnauthorized()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => throw new UnauthorizedAccessException();
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        _hostEnvironmentMock.Setup(e => e.EnvironmentName).Returns("Development");
        context.RequestServices = CreateServiceProvider(_hostEnvironmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task InvokeAsync_WhenKeyNotFoundException_ShouldReturnNotFound()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => throw new KeyNotFoundException();
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        _hostEnvironmentMock.Setup(e => e.EnvironmentName).Returns("Development");
        context.RequestServices = CreateServiceProvider(_hostEnvironmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task InvokeAsync_WhenGenericException_ShouldReturnInternalServerError()
    {
        // Arrange
        RequestDelegate next = (HttpContext ctx) => throw new InvalidOperationException("Test error");
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        _hostEnvironmentMock.Setup(e => e.EnvironmentName).Returns("Development");
        context.RequestServices = CreateServiceProvider(_hostEnvironmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task InvokeAsync_InProduction_ShouldNotExposeErrorDetails()
    {
        // Arrange
        var errorMessage = "Sensitive internal error";
        RequestDelegate next = (HttpContext ctx) => throw new Exception(errorMessage);
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Setup mock to return false for IsDevelopment (production environment)
        var mockHostEnvironment = new Mock<IHostEnvironment>();
        mockHostEnvironment.Setup(e => e.EnvironmentName).Returns("Production");
        // Note: We can't mock extension methods, so we just use EnvironmentName

        var serviceProvider = new ServiceCollection()
            .AddSingleton<IHostEnvironment>(mockHostEnvironment.Object)
            .BuildServiceProvider();

        context.RequestServices = serviceProvider;

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(context.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        // In production (when not Development), error details should not be exposed
        responseBody.Should().NotContain(errorMessage);
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        RequestDelegate next = (HttpContext ctx) => throw exception;
        var middleware = new GlobalExceptionHandlerMiddleware(next, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        _hostEnvironmentMock.Setup(e => e.EnvironmentName).Returns("Development");
        context.RequestServices = CreateServiceProvider(_hostEnvironmentMock.Object);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    private static IServiceProvider CreateServiceProvider(IHostEnvironment hostEnvironment)
    {
        var services = new ServiceCollection();
        services.AddSingleton(hostEnvironment);
        return services.BuildServiceProvider();
    }
}
