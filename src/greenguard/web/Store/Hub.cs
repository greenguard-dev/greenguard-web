namespace web.Store;

public class Hub
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Uri? Endpoint { get; set; }
    public DeviceId DeviceId { get; set; }
    public bool IsConfirmed { get; set; }
}