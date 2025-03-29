using Marten;
using RazorHx.Results;
using web.Interface;

namespace web.Endpoints;

public static class DefaultEndpoints
{
    public static IEndpointRouteBuilder MapDefault(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/", () => new RazorHxResult<Home>()).RequireAuthorization();
        builder.MapGet("/plants", () => new RazorHxResult<Plants>()).RequireAuthorization();
        builder.MapGet("/settings", () => new RazorHxResult<Settings>()).RequireAuthorization();
        builder.MapPost("/upload", async (HttpContext context) =>
        {
            if (context.Request.HasFormContentType)
            {
                var form = await context.Request.ReadFormAsync();
                var files = form.Files;
            }

            return Results.Ok();
        }).DisableAntiforgery().RequireAuthorization();

        return builder;
    }
}