using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace CashFlowSystem.Domain.Tests.Entities;

public class TransactionTests
{
    [Fact]
    public void Transaction_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var transaction = new Transaction();

        // Assert
        transaction.Id.Should().NotBe(Guid.Empty);
        transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        transaction.IsDeleted.Should().BeFalse();
        transaction.Date.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Transaction_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var transaction = new Transaction();
        var amount = 100.50m;
        var description = "Test transaction";
        var categoryId = Guid.NewGuid();
        var paymentMethodId = Guid.NewGuid();

        // Act
        transaction.Amount = amount;
        transaction.Description = description;
        transaction.Type = TransactionType.Income;
        transaction.CategoryId = categoryId;
        transaction.PaymentMethodId = paymentMethodId;

        // Assert
        transaction.Amount.Should().Be(amount);
        transaction.Description.Should().Be(description);
        transaction.Type.Should().Be(TransactionType.Income);
        transaction.CategoryId.Should().Be(categoryId);
        transaction.PaymentMethodId.Should().Be(paymentMethodId);
    }
}
