using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CashFlowSystem.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.IsActive.Should().BeTrue();
        user.Role.Should().Be(UserRole.Cashier);
        user.Transactions.Should().NotBeNull();
        user.CashRegisters.Should().NotBeNull();
    }

    [Fact]
    public void User_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var user = new User();
        var username = "testuser";
        var email = "test@example.com";
        var passwordHash = "hashedpassword";

        // Act
        user.Username = username;
        user.Email = email;
        user.PasswordHash = passwordHash;
        user.Role = UserRole.Admin;

        // Assert
        user.Username.Should().Be(username);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(UserRole.Admin);
    }
}
