using CashFlowSystem.Application.DTOs.Auth;
using MediatR;

namespace CashFlowSystem.Application.Commands.Auth;

public class LoginCommand : IRequest<AuthResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
