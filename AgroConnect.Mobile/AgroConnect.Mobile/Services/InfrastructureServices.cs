using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

public class SecureStorageService : ISecureStorageService
{
    public async Task SetAsync(string key, string value) => await SecureStorage.Default.SetAsync(key, value);
    public async Task<string?> GetAsync(string key) => await SecureStorage.Default.GetAsync(key);
    public Task RemoveAsync(string key) { SecureStorage.Default.Remove(key); return Task.CompletedTask; }
    public Task ClearAllAsync() { SecureStorage.Default.RemoveAll(); return Task.CompletedTask; }
}

public class ConnectivityService : IConnectivityService
{
    public ConnectivityService()
    {
        Connectivity.Current.ConnectivityChanged += (_, args) =>
            ConnectivityChanged?.Invoke(this, args.NetworkAccess == NetworkAccess.Internet);
    }

    public bool IsConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
    public event EventHandler<bool>? ConnectivityChanged;
}
