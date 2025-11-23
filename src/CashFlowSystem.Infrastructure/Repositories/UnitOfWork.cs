using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using CashFlowSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace CashFlowSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy initialization of repositories
    private IRepository<User>? _users;
    private IRepository<Category>? _categories;
    private IRepository<PaymentMethod>? _paymentMethods;
    private IRepository<Transaction>? _transactions;
    private IRepository<CashRegister>? _cashRegisters;
    private IRepository<Budget>? _budgets;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository properties with lazy initialization
    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<PaymentMethod> PaymentMethods => _paymentMethods ??= new Repository<PaymentMethod>(_context);
    public IRepository<Transaction> Transactions => _transactions ??= new Repository<Transaction>(_context);
    public IRepository<CashRegister> CashRegisters => _cashRegisters ??= new Repository<CashRegister>(_context);
    public IRepository<Budget> Budgets => _budgets ??= new Repository<Budget>(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
