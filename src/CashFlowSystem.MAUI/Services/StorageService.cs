namespace CashFlowSystem.MAUI.Services;

public class StorageService : IStorageService
{
    public async Task<string?> GetAsync(string key)
    {
        return await SecureStorage.Default.GetAsync(key);
    }

    public async Task SetAsync(string key, string value)
    {
        await SecureStorage.Default.SetAsync(key, value);
    }

    public async Task RemoveAsync(string key)
    {
        SecureStorage.Default.Remove(key);
        await Task.CompletedTask;
    }
}
