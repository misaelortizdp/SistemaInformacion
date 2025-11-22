using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Application.Queries.PaymentMethods;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using MediatR;

namespace CashFlowSystem.Application.Handlers.PaymentMethods;

public class GetAllPaymentMethodsQueryHandler : IRequestHandler<GetAllPaymentMethodsQuery, IEnumerable<PaymentMethodDto>>
{
    private readonly IRepository<PaymentMethod> _paymentMethodRepository;
    private readonly IMapper _mapper;

    public GetAllPaymentMethodsQueryHandler(IRepository<PaymentMethod> paymentMethodRepository, IMapper mapper)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetAllPaymentMethodsQuery request, CancellationToken cancellationToken)
    {
        var paymentMethods = await _paymentMethodRepository.GetAllAsync(cancellationToken);
        var activePaymentMethods = paymentMethods.Where(pm => pm.IsActive).OrderBy(pm => pm.Name);
        return _mapper.Map<IEnumerable<PaymentMethodDto>>(activePaymentMethods);
    }
}
