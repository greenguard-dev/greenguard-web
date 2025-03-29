using web.Store;

namespace web.Endpoints.Hub.Requests;

public class CreateHubRequest
{
    public string? Name { get; set; }
    public DeviceId DeviceId { get; set; }
    public required string Endpoint { get; set; }
    public required string Ssid { get; set; }
    public required string Password { get; set; }
}