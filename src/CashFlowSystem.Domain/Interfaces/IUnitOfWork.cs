using CashFlowSystem.Domain.Entities;

namespace CashFlowSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repository properties
    IRepository<User> Users { get; }
    IRepository<Category> Categories { get; }
    IRepository<PaymentMethod> PaymentMethods { get; }
    IRepository<Transaction> Transactions { get; }
    IRepository<CashRegister> CashRegisters { get; }
    IRepository<Budget> Budgets { get; }

    // Unit of Work methods
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
