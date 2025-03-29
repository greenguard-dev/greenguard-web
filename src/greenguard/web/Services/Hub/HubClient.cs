namespace web.Services.Hub;

public class HubClient : IHubClient
{
    private readonly HttpClient _httpClient;

    public HubClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ScanForDevicesAsync(Store.Hub hub)
    {
        var endpoint = hub.Endpoint ?? throw new InvalidOperationException("Hub endpoint is not set");
        _httpClient.BaseAddress = endpoint;
        
        const string path = "/devices/scan";
        
        var response = await _httpClient.GetAsync(path);
        
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Failed to scan for devices: {response.ReasonPhrase}");
        }
        
        var content = await response.Content.ReadFromJsonAsync<List<Device>>();
        
        if (content == null)
        {
            throw new InvalidOperationException("Failed to deserialize scan response");
        }
    }
    
    internal class Device
    {
        public string Address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DeviceType Type { get; set; }
    }
    
    public enum DeviceType
    {
        MiFlora
    }
}