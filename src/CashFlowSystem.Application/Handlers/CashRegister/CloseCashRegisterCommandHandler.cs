using CashFlowSystem.Application.Commands.CashRegister;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.CashRegister;

public class CloseCashRegisterCommandHandler : IRequestHandler<CloseCashRegisterCommand, CashRegisterDto>
{
    private readonly IRepository<Domain.Entities.CashRegister> _cashRegisterRepository;
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseCashRegisterCommandHandler(
        IRepository<Domain.Entities.CashRegister> cashRegisterRepository,
        IRepository<Transaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _cashRegisterRepository = cashRegisterRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CashRegisterDto> Handle(CloseCashRegisterCommand request, CancellationToken cancellationToken)
    {
        var cashRegister = await _cashRegisterRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cashRegister == null)
        {
            throw new InvalidOperationException($"Cash register with ID {request.Id} not found");
        }

        if (cashRegister.Status == CashRegisterStatus.Closed)
        {
            throw new InvalidOperationException("Cash register is already closed");
        }

        // Calculate expected balance
        var transactions = await _transactionRepository.GetAllAsync(cancellationToken);
        var cashRegisterTransactions = transactions.Where(t =>
            t.CashRegisterId == cashRegister.Id).ToList();

        var totalIncome = cashRegisterTransactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount);

        var totalExpense = cashRegisterTransactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount);

        var expectedBalance = cashRegister.OpeningBalance + totalIncome - totalExpense;

        // Close cash register
        cashRegister.ClosingDate = DateTime.UtcNow;
        cashRegister.ClosingBalance = request.ClosingBalance;
        cashRegister.ExpectedBalance = expectedBalance;
        cashRegister.Difference = request.ClosingBalance - expectedBalance;
        cashRegister.Status = CashRegisterStatus.Closed;
        if (!string.IsNullOrEmpty(request.Notes))
        {
            cashRegister.Notes += $"\n{request.Notes}";
        }

        await _cashRegisterRepository.UpdateAsync(cashRegister, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CashRegisterDto
        {
            Id = cashRegister.Id,
            OpeningDate = cashRegister.OpeningDate,
            ClosingDate = cashRegister.ClosingDate,
            OpeningBalance = cashRegister.OpeningBalance,
            ClosingBalance = cashRegister.ClosingBalance,
            ExpectedBalance = cashRegister.ExpectedBalance,
            Difference = cashRegister.Difference,
            Status = cashRegister.Status.ToString(),
            Notes = cashRegister.Notes,
            UserId = cashRegister.UserId
        };
    }
}
