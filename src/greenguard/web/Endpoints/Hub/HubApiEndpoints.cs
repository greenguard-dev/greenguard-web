using Microsoft.AspNetCore.Mvc;
using web.Endpoints.Hub.Requests;
using web.Services.Hub;

namespace web.Endpoints.Hub;

public static class HubApiEndpoints
{
    public static IEndpointRouteBuilder MapApiHub(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("api/hub");

        hubGroup.MapGet("/", async (IHubService hubService) =>
        {
            await hubService.GetHubsAsync();
            return Results.Ok();
        });

        hubGroup.MapGet("/{id:guid}", async (Guid id, IHubService hubService) =>
        {
            await hubService.GetHubAsync(id);
            return Results.Ok();
        });

        hubGroup.MapPost("/",
            async ([FromForm] CreateHubRequest createHubRequest, IHubService hubService, HttpContext context) =>
            {
                await hubService.RegisterHubAsync(Guid.NewGuid(), createHubRequest.Name, createHubRequest.DeviceId);
                
                context.Response.Headers["HX-Trigger"] = "hub-added";
                
                return Results.Ok();
            }).DisableAntiforgery();

        hubGroup.MapDelete("/{id:guid}", async (Guid id, IHubService hubService) =>
        {
            await hubService.DeleteHubAsync(id);
            return Results.Ok();
        });

        hubGroup.MapPatch("/{id:guid}", async (Guid id, IHubService hubService) =>
        {
            await hubService.ConfirmHubAsync(id);
            return Results.Ok();
        });

        hubGroup.MapGet("/{id:guid}/scan", async (Guid id, IHubService hubService) =>
        {
            await hubService.ScanForDevicesAsync(id);
            return Results.Ok();
        });

        return builder;
    }
}