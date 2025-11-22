namespace CashFlowSystem.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(Guid userId, string username, string email, string role);
    string? ValidateToken(string token);
}
