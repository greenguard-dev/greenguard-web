namespace web.Services.Version;

public class VersionInfo : IVersionInfo
{
    public string Version { get; set; }
    public string? InformationalVersion { get; set;}
}