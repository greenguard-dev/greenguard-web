﻿using Marten;
using RazorHx.Results;
using web.Interface.Hub;

namespace web.Endpoints.Hub;

public static class HubEndpoints
{
    public static IEndpointRouteBuilder MapHubEndpoints(this IEndpointRouteBuilder builder)
    {
        var hubGroup = builder.MapGroup("hubs")
            .ExcludeFromDescription()
            .RequireAuthorization();;

        hubGroup.MapGet("/", async (IDocumentSession documentSession) =>
        {
            var hubs = await documentSession.Query<Store.Hub>().ToListAsync();
            return new RazorHxResult<Hubs>(new { HubList = hubs });
        });

        hubGroup.MapGet("/add", () => new RazorHxResult<AddHub>());

        return builder;
    }
}