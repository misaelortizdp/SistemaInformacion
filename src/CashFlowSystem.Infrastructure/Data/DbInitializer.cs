using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Default Categories
        if (!context.Categories.Any())
        {
            var incomeCategories = new List<Category>
            {
                new Category { Name = "Ventas", Type = TransactionType.Income, Color = "#4CAF50", Icon = "currency-usd" },
                new Category { Name = "Servicios", Type = TransactionType.Income, Color = "#2196F3", Icon = "briefcase" },
                new Category { Name = "Otros Ingresos", Type = TransactionType.Income, Color = "#9C27B0", Icon = "cash-plus" }
            };

            var expenseCategories = new List<Category>
            {
                new Category { Name = "Compras", Type = TransactionType.Expense, Color = "#F44336", Icon = "cart" },
                new Category { Name = "Sueldos", Type = TransactionType.Expense, Color = "#FF9800", Icon = "account-cash" },
                new Category { Name = "Servicios Básicos", Type = TransactionType.Expense, Color = "#795548", Icon = "home" },
                new Category { Name = "Alquiler", Type = TransactionType.Expense, Color = "#607D8B", Icon = "office-building" },
                new Category { Name = "Otros Gastos", Type = TransactionType.Expense, Color = "#9E9E9E", Icon = "cash-minus" }
            };

            await context.Categories.AddRangeAsync(incomeCategories);
            await context.Categories.AddRangeAsync(expenseCategories);
        }

        // Seed Default Payment Methods
        if (!context.PaymentMethods.Any())
        {
            var paymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Name = "Efectivo", Type = PaymentMethodType.Cash },
                new PaymentMethod { Name = "Tarjeta de Débito", Type = PaymentMethodType.DebitCard },
                new PaymentMethod { Name = "Tarjeta de Crédito", Type = PaymentMethodType.CreditCard },
                new PaymentMethod { Name = "Transferencia Bancaria", Type = PaymentMethodType.BankTransfer },
                new PaymentMethod { Name = "Cheque", Type = PaymentMethodType.Check }
            };

            await context.PaymentMethods.AddRangeAsync(paymentMethods);
        }

        await context.SaveChangesAsync();
    }
}
