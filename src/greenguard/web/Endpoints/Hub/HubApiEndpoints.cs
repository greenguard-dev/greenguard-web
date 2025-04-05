using Microsoft.AspNetCore.Mvc;
using web.Endpoints.Hub.Requests;
using web.Services.Hub;

namespace web.Endpoints.Hub;

public static class HubApiEndpoints
{
    public static IEndpointRouteBuilder MapApiHubEndpoints(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("api/hubs")
            .WithTags("Hubs");

        hubGroup.MapGet("/", async (IHubService hubService) =>
        {
            await hubService.GetHubsAsync();
            return Results.Ok();
        }).WithName("GetHubs");

        hubGroup.MapGet("/{id:guid}", async (Guid id, IHubService hubService) =>
        {
            await hubService.GetHubAsync(id);
            return Results.Ok();
        }).WithName("GetHub");

        hubGroup.MapPost("/",
            async ([FromForm] CreateHubRequest createHubRequest, IHubService hubService, HttpContext context) =>
            {
                await hubService.RegisterHubAsync(Guid.NewGuid(), createHubRequest.Name, createHubRequest.DeviceId);

                context.Response.Headers["HX-Trigger"] = "hub-added";

                return Results.Ok();
            }).WithName("AddHub")
            .DisableAntiforgery();

        hubGroup.MapDelete("/{id:guid}", async (Guid id, IHubService hubService) =>
        {
            await hubService.DeleteHubAsync(id);
            return Results.Ok();
        }).WithName("DeleteHub");

        hubGroup.MapGet("/{id:guid}/scan", async (Guid id, IHubService hubService) =>
        {
            var devices = hubService.ScanForDevicesAsync(id);
            var deviceList = new List<HubClient.Device>();

            await foreach (var device in devices)
            {
                deviceList.Add(device);
            }

            return Results.Ok(deviceList);
        }).WithName("ScanDevicesForHub");

        hubGroup.MapGet("/scan", async (IHubService hubService) =>
        {
            var devices = hubService.ScanForDevicesAsync();
            var deviceList = new List<HubClient.Device>();

            await foreach (var device in devices)
            {
                deviceList.Add(device);
            }

            return Results.Ok(deviceList);
        }).WithName("ScanDevicesGlobally");

        hubGroup.MapGet("/{id:guid}/health", async (Guid id, IHubService hubService, HttpContext context) =>
        {
            var hubIpAddress = context.Request.Headers["x-greenguard-hub-ip"];
            await hubService.HealthCheckAsync(id, hubIpAddress);
            return Results.Ok();
        }).WithName("HubHealthCheck");

        return builder;
    }
}