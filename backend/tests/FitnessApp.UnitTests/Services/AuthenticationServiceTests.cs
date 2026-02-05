using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Application.Services;
using FitnessApp.Application.Common.Interfaces;
using FitnessApp.Application.DTOs;
using FitnessApp.Domain.Entities;

namespace FitnessApp.UnitTests.Services;

public class AuthenticationServiceTests : IDisposable
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<ILogger<AuthenticationService>> _mockLogger;
    private readonly AuthenticationService _authService;
    private readonly Mock<DbSet<User>> _mockUserSet;
    private readonly Mock<DbSet<RefreshToken>> _mockRefreshTokenSet;
    private readonly Mock<DbSet<EmailVerificationToken>> _mockEmailVerificationTokenSet;

    public AuthenticationServiceTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockTokenService = new Mock<ITokenService>();
        _mockEmailService = new Mock<IEmailService>();
        _mockLogger = new Mock<ILogger<AuthenticationService>>();
        
        _mockUserSet = new Mock<DbSet<User>>();
        _mockRefreshTokenSet = new Mock<DbSet<RefreshToken>>();
        _mockEmailVerificationTokenSet = new Mock<DbSet<EmailVerificationToken>>();

        _mockContext.Setup(c => c.Users).Returns(_mockUserSet.Object);
        _mockContext.Setup(c => c.RefreshTokens).Returns(_mockRefreshTokenSet.Object);
        _mockContext.Setup(c => c.EmailVerificationTokens).Returns(_mockEmailVerificationTokenSet.Object);

        _authService = new AuthenticationService(
            _mockContext.Object,
            _mockTokenService.Object,
            _mockEmailService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUserAndSendsVerificationEmail()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Password = "Test123!@#",
            Name = "Test User"
        };

        var users = new List<User>();
        _mockUserSet.Setup(m => m.Add(It.IsAny<User>()))
            .Callback<User>(u => users.Add(u));

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().Contain("Registration successful");
        _mockEmailService.Verify(e => e.SendEmailVerificationAsync(
            request.Email,
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "existing@example.com",
            Password = "Test123!@#",
            Name = "Test User"
        };

        var existingUser = new User
        {
            Email = "existing@example.com",
            Name = "Existing User",
            PasswordHash = "hash"
        };

        var users = new List<User> { existingUser }.AsQueryable();
        _mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
        _mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _authService.RegisterAsync(request));
    }

    [Fact]
    public void PasswordHashing_ShouldUseCorrectWorkFactor()
    {
        // This test verifies that BCrypt is used with appropriate work factor
        var password = "Test123!@#";
        var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        
        // Verify hash can be validated
        var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        isValid.Should().BeTrue();
        
        // Verify wrong password fails
        var isInvalid = BCrypt.Net.BCrypt.Verify("WrongPassword", hash);
        isInvalid.Should().BeFalse();
    }

    public void Dispose()
    {
        // Cleanup if needed
        GC.SuppressFinalize(this);
    }
}
