using Marten;
using RazorHx.Results;
using web.Interface;

namespace web.Endpoints;

public static class DefaultEndpoints
{
    public static IEndpointRouteBuilder MapDefault(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", () => new RazorHxResult<Home>()).RequireAuthorization();
        builder.MapGet("/settings", () => new RazorHxResult<Settings>()).RequireAuthorization();

        return builder;
    }
}