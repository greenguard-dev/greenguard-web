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

    public async Task HealthCheckAsync(Guid id, string? hubIp)
    {
        var hub = await _documentSession.LoadAsync<Store.Hub>(id);

        if (hub == null)
        {
            throw new InvalidOperationException("Hub not found");
        }

        if (hubIp != null)
        {
            hub.Endpoint = $"http://{hubIp}";
        }

        hub.LastHealthCheck = DateTime.UtcNow;
        _documentSession.Store(hub);

        await _documentSession.SaveChangesAsync();
    }

    public async Task ScanForDevicesAsync(Guid id)
    {
        var hub = await _documentSession.LoadAsync<Store.Hub>(id);

        if (hub == null)
        {
            throw new InvalidOperationException("Hub not found");
        }

        await _hubClient.ScanForDevicesAsync(hub);
    }
}