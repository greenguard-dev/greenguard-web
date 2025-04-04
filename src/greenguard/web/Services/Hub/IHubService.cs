using web.Store;

namespace web.Services.Hub;

public interface IHubService
{
    Task<IReadOnlyList<Store.Hub>> GetHubsAsync();
    Task<Store.Hub?> GetHubAsync(Guid id);
    Task RegisterHubAsync(Guid id, string? name, DeviceId deviceId);
    Task DeleteHubAsync(Guid id);
    Task HealthCheckAsync(Guid id, string? hubIpAddress);
    IAsyncEnumerable<HubClient.Device> ScanForDevicesAsync(Guid id);
    IAsyncEnumerable<HubClient.Device> ScanForDevicesAsync();
}