using CashFlowSystem.MAUI.Models;

namespace CashFlowSystem.MAUI.Services;

public interface IAuthService
{
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string username, string email, string password);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<AuthResponse?> GetCurrentUserAsync();
}
