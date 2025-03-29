using web.Store;

namespace web.Services.Hub;

public interface IHubService
{
    Task<IReadOnlyList<Store.Hub>> GetHubsAsync();
    Task<Store.Hub?> GetHubAsync(Guid id);
    Task RegisterHubAsync(Guid id, string? name, DeviceId deviceId);
    Task ConfirmHubAsync(Guid id);
    Task DeleteHubAsync(Guid id);
    Task ScanForDevicesAsync(Guid id);
}