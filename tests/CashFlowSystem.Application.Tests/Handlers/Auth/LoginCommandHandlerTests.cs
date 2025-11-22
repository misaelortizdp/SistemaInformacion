using CashFlowSystem.Application.Commands.Auth;
using CashFlowSystem.Application.Handlers.Auth;
using CashFlowSystem.Application.Interfaces;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace CashFlowSystem.Application.Tests.Handlers.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IRepository<User>> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IRepository<User>>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtService>();
        _configurationMock = new Mock<IConfiguration>();

        _configurationMock.Setup(c => c["JwtSettings:ExpirationMinutes"]).Returns("60");

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "hashedpassword",
            Role = UserRole.Admin,
            IsActive = true
        };

        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _passwordHasherMock
            .Setup(p => p.VerifyPassword("password123", "hashedpassword"))
            .Returns(true);

        _jwtServiceMock
            .Setup(j => j.GenerateToken(userId, "testuser", "test@example.com", "Admin"))
            .Returns("fake-jwt-token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Email.Should().Be("test@example.com");
        result.Token.Should().Be("fake-jwt-token");
    }

    [Fact]
    public async Task Handle_InvalidEmail_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "invalid@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            IsActive = true
        };

        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        _userRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        _passwordHasherMock
            .Setup(p => p.VerifyPassword("wrongpassword", "hashedpassword"))
            .Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _handler.Handle(command, CancellationToken.None));
    }
}
