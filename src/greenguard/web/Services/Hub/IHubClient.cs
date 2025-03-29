namespace web.Services.Hub;

public interface IHubClient
{
    Task ScanForDevicesAsync(Store.Hub hub);
}