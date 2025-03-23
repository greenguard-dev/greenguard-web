namespace web.Services.Version;

public interface IVersionInfo
{
    public string Version { get; }
    public string? InformationalVersion { get; }
}