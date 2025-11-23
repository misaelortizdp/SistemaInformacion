using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Transactions;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Transactions;

public class SearchTransactionsHandler : IRequestHandler<SearchTransactionsQuery, IEnumerable<TransactionDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchTransactionsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TransactionDto>> Handle(SearchTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _unitOfWork.Transactions.GetAllAsync();
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var paymentMethods = await _unitOfWork.PaymentMethods.GetAllAsync();

        // Apply filters
        var query = transactions.Where(t => !t.IsDeleted).AsQueryable();

        // User filter
        if (request.UserId.HasValue)
        {
            query = query.Where(t => t.UserId == request.UserId.Value);
        }

        // Date range filter
        if (request.StartDate.HasValue)
        {
            query = query.Where(t => t.Date >= request.StartDate.Value);
        }
        if (request.EndDate.HasValue)
        {
            query = query.Where(t => t.Date <= request.EndDate.Value);
        }

        // Type filter
        if (!string.IsNullOrEmpty(request.Type) && Enum.TryParse<TransactionType>(request.Type, out var type))
        {
            query = query.Where(t => t.Type == type);
        }

        // Text search (descripción, notas, referencia)
        if (!string.IsNullOrWhiteSpace(request.SearchText))
        {
            var searchLower = request.SearchText.ToLower();
            query = query.Where(t =>
                (t.Description != null && t.Description.ToLower().Contains(searchLower)) ||
                (t.Notes != null && t.Notes.ToLower().Contains(searchLower)) ||
                (t.ReferenceNumber != null && t.ReferenceNumber.ToLower().Contains(searchLower))
            );
        }

        // Amount range filter
        if (request.MinAmount.HasValue)
        {
            query = query.Where(t => t.Amount >= request.MinAmount.Value);
        }
        if (request.MaxAmount.HasValue)
        {
            query = query.Where(t => t.Amount <= request.MaxAmount.Value);
        }

        // Category filter
        if (request.CategoryIds != null && request.CategoryIds.Any())
        {
            query = query.Where(t => request.CategoryIds.Contains(t.CategoryId));
        }

        // Payment method filter
        if (request.PaymentMethodIds != null && request.PaymentMethodIds.Any())
        {
            query = query.Where(t => request.PaymentMethodIds.Contains(t.PaymentMethodId));
        }

        // Ordering
        query = (request.OrderBy?.ToLower()) switch
        {
            "amount" => request.Descending
                ? query.OrderByDescending(t => t.Amount)
                : query.OrderBy(t => t.Amount),
            "description" => request.Descending
                ? query.OrderByDescending(t => t.Description)
                : query.OrderBy(t => t.Description),
            _ => request.Descending
                ? query.OrderByDescending(t => t.Date)
                : query.OrderBy(t => t.Date)
        };

        // Paginación
        if (request.Skip.HasValue)
        {
            query = query.Skip(request.Skip.Value);
        }
        if (request.Take.HasValue)
        {
            query = query.Take(request.Take.Value);
        }

        // Map to DTOs
        var result = query.Select(t => new TransactionDto
        {
            Id = t.Id,
            Amount = t.Amount,
            Description = t.Description,
            Date = t.Date,
            Type = t.Type.ToString(),
            CategoryId = t.CategoryId,
            CategoryName = categories.FirstOrDefault(c => c.Id == t.CategoryId)?.Name ?? "",
            PaymentMethodId = t.PaymentMethodId,
            PaymentMethodName = paymentMethods.FirstOrDefault(p => p.Id == t.PaymentMethodId)?.Name ?? "",
            ReferenceNumber = t.ReferenceNumber,
            Notes = t.Notes
        }).ToList();

        return result;
    }
}
