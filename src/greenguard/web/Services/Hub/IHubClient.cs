namespace web.Services.Hub;

public interface IHubClient
{
    IAsyncEnumerable<HubClient.Device> ScanForDevicesAsync(Store.Hub hub);
}