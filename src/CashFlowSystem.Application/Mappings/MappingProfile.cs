using AutoMapper;
using CashFlowSystem.Application.DTOs;
using CashFlowSystem.Domain.Entities;

namespace CashFlowSystem.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Transaction mappings
        CreateMap<Transaction, TransactionDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethod.Name));

        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // PaymentMethod mappings
        CreateMap<PaymentMethod, PaymentMethodDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

        // User mappings
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}
