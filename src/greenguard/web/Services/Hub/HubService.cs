using Marten;
using web.Store;

namespace web.Services.Hub;

public class HubService : IHubService
{
    private readonly IDocumentSession _documentSession;
    private readonly IHubClient _hubClient;

    public HubService(IDocumentSession documentSession, IHubClient hubClient)
    {
        _documentSession = documentSession;
        _hubClient = hubClient;
    }

    public async Task<IReadOnlyList<Store.Hub>> GetHubsAsync()
    {
        var hubs = await _documentSession.Query<Store.Hub>().ToListAsync();
        return hubs;
    }

    public async Task<Store.Hub?> GetHubAsync(Guid id)
    {
        var hub = await _documentSession.LoadAsync<Store.Hub>(id);
        return hub;
    }

    public Task RegisterHubAsync(Guid id, string name, DeviceId deviceId)
    {
        var hub = new Store.Hub
        {
            Id = id,
            Name = name,
            DeviceId = deviceId
        };

        _documentSession.Store(hub);
        return _documentSession.SaveChangesAsync();
    }

    public Task DeleteHubAsync(Guid id)
    {
        _documentSession.Delete<Store.Hub>(id);
        return _documentSession.SaveChangesAsync();
    }

    public async Task HealthCheckAsync(Guid id, string? hubIpAddress)
    {
        var hub = await _documentSession.LoadAsync<Store.Hub>(id);

        if (hub == null)
        {
            throw new InvalidOperationException("Hub not found");
        }

        if (hubIpAddress != null && hub.IpAddress != hubIpAddress)
        {
            hub.IpAddress = hubIpAddress;
        }

        hub.LastHealthCheck = DateTime.UtcNow;
        _documentSession.Update(hub);

        await _documentSession.SaveChangesAsync();
    }

    public async IAsyncEnumerable<HubClient.Device> ScanForDevicesAsync(Guid id)
    {
        var hub = await _documentSession.LoadAsync<Store.Hub>(id);

        if (hub == null)
        {
            throw new InvalidOperationException("Hub not found");
        }

        var devices = _hubClient.ScanForDevicesAsync(hub);
        await foreach (var device in devices)
        {
            yield return device;
        }
    }

    public async IAsyncEnumerable<HubClient.Device> ScanForDevicesAsync()
    {
        var hubs = _documentSession.Query<Store.Hub>().ToList();

        foreach (var devices in hubs.Select(hub => _hubClient.ScanForDevicesAsync(hub)))
        {
            await foreach (var device in devices)
            {
                yield return device;
            }
        }
    }
}