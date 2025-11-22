using CashFlowSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlowSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<PaymentMethod>().HasQueryFilter(pm => !pm.IsDeleted);
        modelBuilder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);
        modelBuilder.Entity<CashRegister>().HasQueryFilter(cr => !cr.IsDeleted);
    }
}
