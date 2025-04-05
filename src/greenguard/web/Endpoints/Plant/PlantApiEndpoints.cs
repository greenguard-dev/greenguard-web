using Microsoft.AspNetCore.Mvc;
using web.Endpoints.Plant.Requests;
using web.Services.Hub;
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

        plantGroup.MapPost("/{id:guid}/upload", async (Guid id, HttpContext context, IPlantService plantService) =>
            {
                if (!context.Request.HasFormContentType) return Results.NoContent();

                var form = await context.Request.ReadFormAsync();
                var files = form.Files;

                if (files.Count == 0) return Results.NoContent();

                var file = files[0];
                var isPng = file.ContentType == "image/png";
                var isJpg = file.ContentType == "image/jpeg";

                if (!isPng && !isJpg) return Results.NoContent();

                var filePath = Path.Combine("wwwroot", "plants");

                if (isPng)
                    filePath = Path.Combine(filePath, $"{id}.png");
                else if (isJpg)
                    filePath = Path.Combine(filePath, $"{id}.jpg");

                var directoryPath = Path.GetDirectoryName(filePath);

                if (directoryPath is not null && !Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var plant = await plantService.GetPlantAsync(id);
                if (plant is null) return Results.NotFound();

                if (isPng)
                    plant.ImageUrl = $"/plants/{id}.png";
                else if (isJpg)
                    plant.ImageUrl = $"/plants/{id}.jpg";

                await plantService.UpdatePlantAsync(plant);

                context.Response.Headers["HX-Trigger"] = "plant-added";

                return Results.NoContent();
            }).WithName("UploadPlantImage")
            .DisableAntiforgery();

        return builder;
    }
}