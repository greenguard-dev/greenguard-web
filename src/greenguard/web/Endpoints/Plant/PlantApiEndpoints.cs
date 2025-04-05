using Microsoft.AspNetCore.Mvc;
using web.Endpoints.Plant.Requests;
using web.Services.Hub;
using web.Services.Image;
using web.Services.Plant;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;

namespace web.Endpoints.Plant;

public static class PlantApiEndpoints
{
    public static IEndpointRouteBuilder MapPlantApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var plantGroup = builder.MapGroup("api/plants")
            .WithTags("Plants");

        plantGroup.MapGet("/", async (IHubService hubService) =>
        {
            await hubService.GetHubsAsync();
            return Results.Ok();
        }).WithName("GetPlants");

        plantGroup.MapPost("/manually",
                async ([FromForm] AddPlantRequest addPlantRequest, IPlantService plantService, HttpContext context) =>
                {
                    await plantService.AddPlantAsync(Guid.NewGuid(), addPlantRequest.ManuallyName);
                    context.Response.Headers["HX-Trigger"] = "plant-added";
                    return Results.Ok();
                }).WithName("AddPlantManually")
            .DisableAntiforgery();

        plantGroup.MapPost("/sensor",
                async (IPlantService plantService, HttpContext context) =>
                {
                    var form = await context.Request.ReadFormAsync();

                    foreach (var formKey in form.Keys)
                    {
                        var split = formKey.Split('-');
                        if (split.Length != 2) continue;

                        var address = split[0];
                        var name = split[1];
                        await plantService.AddPlantSensorAsync(Guid.NewGuid(), name, address);
                    }

                    context.Response.Headers["HX-Trigger"] = "plant-added";
                    return Results.Ok();
                }).WithName("AddPlantSensor")
            .DisableAntiforgery();

        plantGroup.MapPost("/{id:guid}/upload",
                async (Guid id, IFormFile image, HttpContext context, IPlantService plantService) =>
                {
                    if (!context.Request.HasFormContentType) return Results.NoContent();
                    if (image.Length == 0) return Results.NoContent();

                    var plant = await plantService.GetPlantAsync(id);
                    if (plant is null) return Results.NotFound();

                    var folder = Path.Combine("wwwroot", "thumbnails");
                    var fileName = $"{id}-{DateTime.UtcNow.Ticks}.webp";

                    if (Directory.Exists(folder) == false)
                        Directory.CreateDirectory(folder);

                    foreach (var file in Directory.EnumerateFiles(folder, $"{id}-*.webp"))
                    {
                        File.Delete(file);
                    }

                    await ImageResizer.CreateThumbnailAsync(image, 500, 500, folder, fileName);

                    plant.ImageUrl = $"/thumbnails/{fileName}";

                    await plantService.UpdatePlantAsync(plant);

                    context.Response.Headers["HX-Trigger"] = "plant-added";

                    return Results.NoContent();
                }).WithName("UploadPlantImage")
            .DisableAntiforgery();

        return builder;
    }
}