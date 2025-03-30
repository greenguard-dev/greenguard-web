using Marten;
using RazorHx.Results;
using web.Interface;
using web.Interface.Hub;
using web.Services;
using web.Store;

namespace web.Endpoints.Plant.Hub;

public static class PlantEndpoints
{
    public static IEndpointRouteBuilder MapPlantEndpoints(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("plants");
        hubGroup.RequireAuthorization();

        hubGroup.MapGet("/", async (IDocumentSession documentSession) =>
        {
            var plants = new List<(Store.Plant plant, PlantMeasurement measurement)>();

            var random = new Random();

            plants.AddRange([
                (new Store.Plant { Id = Guid.NewGuid(), Name = NameGenerator.GenerateName(), ImageUrl = "/monstera.jpeg"}, new PlantMeasurement
                {
                    Id = Guid.NewGuid(), Items =
                        new Dictionary<string, int>
                        {
                            { "Humidity", random.Next(0, 100) },
                            { "Temperature", random.Next(0, 100) },
                            { "Light", random.Next(0, 100) },
                            { "Fertility", random.Next(0, 100) },
                        }
                }),
            ]);

            return new RazorHxResult<Plants>(new { PlantsList = plants });
        });

        hubGroup.MapGet("/register", () => new RazorHxResult<RegisterHub>());

        return builder;
    }
}