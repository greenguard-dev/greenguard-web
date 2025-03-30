using Marten;
using RazorHx.Results;
using web.Interface.Hub;

namespace web.Endpoints.Hub;

public static class HubEndpoints
{
    public static IEndpointRouteBuilder MapHubEndpoints(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("hubs");
        hubGroup.RequireAuthorization();

        hubGroup.MapGet("/", async (IDocumentSession documentSession) =>
        {
            var hubs = await documentSession.Query<Store.Hub>().ToListAsync();
            return new RazorHxResult<Hubs>(new { HubList = hubs });
        });

        hubGroup.MapGet("/register", () => new RazorHxResult<RegisterHub>());

        return builder;
    }
}