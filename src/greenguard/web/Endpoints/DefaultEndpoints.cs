using RazorHx.Results;
using web.Interface;

namespace web.Endpoints;

public static class DefaultEndpoints
{
    public static IEndpointRouteBuilder MapDefault(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", () => new RazorHxResult<Overview>());
        builder.MapGet("/plants", () => new RazorHxResult<Plants>());
        builder.MapGet("/hubs", () => new RazorHxResult<Hubs>());
        builder.MapGet("/settings", () => new RazorHxResult<Settings>());
        builder.MapGet("/login", () => new RazorHxResult<Login>());

        return builder;
    }
}