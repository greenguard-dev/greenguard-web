using Marten;
using RazorHx.Results;
using web.Interface;
using web.Interface.Hub;
using web.Interface.Plant;
using web.Services;
using web.Services.Hub;
using web.Services.Plant;
using web.Store;

namespace web.Endpoints.Plant;

public static class PlantEndpoints
{
    public static IEndpointRouteBuilder MapPlantEndpoints(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("plants");
        hubGroup.RequireAuthorization();

        hubGroup.MapGet("/", async (IPlantService plantService) =>
        {
            var plants = await plantService.GetPlantsAsync();
            var plantMeasurements = new List<PlantMeasurement>();

            var random = new Random();

            plantMeasurements.AddRange([
                new PlantMeasurement
                {
                    Id = Guid.NewGuid(),
                    PlantId = Guid.Empty,
                    Items = new Dictionary<string, int>
                    {
                        { "Humidity", random.Next(0, 100) },
                        { "Temperature", random.Next(0, 100) },
                        { "Light", random.Next(0, 100) },
                        { "Fertility", random.Next(0, 100) },
                    }
                },
                new PlantMeasurement
                {
                    Id = Guid.NewGuid(),
                    PlantId = Guid.Empty,
                    Items = new Dictionary<string, int>
                    {
                        { "Humidity", random.Next(0, 100) },
                        { "Temperature", random.Next(0, 100) },
                        { "Light", random.Next(0, 100) },
                        { "Fertility", random.Next(0, 100) },
                    }
                }
            ]);

            var plantsWithMeasurements = new List<(Store.Plant, PlantMeasurement?)>();

            foreach (var plant in plants)
            {
                var measurement = plantMeasurements.FirstOrDefault(m => m.PlantId == plant.Id);
                plantsWithMeasurements.Add(measurement != null ? (plant, measurement) : (plant, null));
            }
            
            plantsWithMeasurements = plantsWithMeasurements.OrderBy(p => p.Item1.Name).ToList();
            
            return new RazorHxResult<Plants>(new { PlantsList = plantsWithMeasurements });
        });

        hubGroup.MapGet("/add", () => new RazorHxResult<AddPlant>());
        hubGroup.MapGet("/scan", async (IHubService hubService) =>
        {
            var devices = hubService.ScanForDevicesAsync();
            var deviceList = new List<HubClient.Device>();

            await foreach (var device in devices)
            {
                deviceList.Add(device);
            }
            
            return new RazorHxResult<PlantSensorScanner>(new { Devices = deviceList });
        });

        return builder;
    }
}