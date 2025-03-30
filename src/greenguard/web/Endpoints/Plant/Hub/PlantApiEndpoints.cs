using web.Services.Hub;

namespace web.Endpoints.Plant.Hub;

public static class PlantApiEndpoints
{
    public static IEndpointRouteBuilder MapPlantApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("api/plants");

        hubGroup.MapGet("/", async (IHubService hubService) =>
        {
            await hubService.GetHubsAsync();
            return Results.Ok();
        });

        return builder;
    }
}