using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.Categories;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Enums;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.Categories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IMapper _mapper;

    public GetAllCategoriesQueryHandler(IRepository<Category> categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrEmpty(request.Type))
        {
            if (Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
            {
                categories = categories.Where(c => c.Type == transactionType);
            }
        }

        var activeCategories = categories.Where(c => c.IsActive).OrderBy(c => c.Name);
        return _mapper.Map<IEnumerable<CategoryDto>>(activeCategories);
    }
}
