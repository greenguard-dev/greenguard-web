namespace web.Store;

public class Hub
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? IpAddress { get; set; }
    public DeviceId DeviceId { get; set; }
    public DateTime? LastHealthCheck { get; set; }
    public bool IsOnline => LastHealthCheck != null && (DateTime.UtcNow - LastHealthCheck.Value).TotalSeconds < 30;
}