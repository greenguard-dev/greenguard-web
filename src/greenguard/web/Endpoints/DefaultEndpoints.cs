using RazorHx.Results;
using web.Interface;

namespace web.Endpoints;

public static class DefaultEndpoints
{
    public static IEndpointRouteBuilder MapDefault(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", () => new RazorHxResult<Overview>());
        builder.MapGet("/plants", () => new RazorHxResult<Plants>());
        builder.MapGet("/settings", () => new RazorHxResult<Settings>());

        return builder;
    }
}