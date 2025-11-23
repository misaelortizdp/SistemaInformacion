using CashFlowSystem.MAUI.Models;
using Newtonsoft.Json;

namespace CashFlowSystem.MAUI.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private readonly IStorageService _storageService;
    private const string TokenKey = "auth_token";
    private const string UserKey = "user_data";

    public AuthService(IApiService apiService, IStorageService storageService)
    {
        _apiService = apiService;
        _storageService = storageService;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _apiService.PostAsync<AuthResponse>("/auth/login", request);

        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            await _storageService.SetAsync(TokenKey, response.Token);
            await _storageService.SetAsync(UserKey, JsonConvert.SerializeObject(response));
            _apiService.SetAuthToken(response.Token);
            return true;
        }

        return false;
    }

    public async Task<bool> RegisterAsync(string username, string email, string password)
    {
        var request = new RegisterRequest
        {
            Username = username,
            Email = email,
            Password = password,
            ConfirmPassword = password
        };

        var response = await _apiService.PostAsync<AuthResponse>("/auth/register", request);

        if (response != null && !string.IsNullOrEmpty(response.Token))
        {
            await _storageService.SetAsync(TokenKey, response.Token);
            await _storageService.SetAsync(UserKey, JsonConvert.SerializeObject(response));
            _apiService.SetAuthToken(response.Token);
            return true;
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        await _storageService.RemoveAsync(TokenKey);
        await _storageService.RemoveAsync(UserKey);
        _apiService.ClearAuthToken();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _storageService.GetAsync(TokenKey);
        if (!string.IsNullOrEmpty(token))
        {
            _apiService.SetAuthToken(token);
            return true;
        }
        return false;
    }

    public async Task<AuthResponse?> GetCurrentUserAsync()
    {
        var userData = await _storageService.GetAsync(UserKey);
        if (!string.IsNullOrEmpty(userData))
        {
            return JsonConvert.DeserializeObject<AuthResponse>(userData);
        }
        return null;
    }
}
