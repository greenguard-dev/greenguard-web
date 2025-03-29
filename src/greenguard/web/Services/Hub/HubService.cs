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
            DeviceId = deviceId,
            IsConfirmed = false
        };

        _documentSession.Store(hub);
        return _documentSession.SaveChangesAsync();
    }

    public async Task ConfirmHubAsync(Guid id)
    {
        var hub = await _documentSession.LoadAsync<Store.Hub>(id);

        if (hub == null)
        {
            throw new InvalidOperationException("Hub not found");
        }

        hub.IsConfirmed = true;
        await _documentSession.SaveChangesAsync();
    }

    public Task DeleteHubAsync(Guid id)
    {
        _documentSession.Delete<Store.Hub>(id);
        return _documentSession.SaveChangesAsync();
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