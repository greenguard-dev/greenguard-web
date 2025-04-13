using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using RazorHx.Results;
using web.Endpoints.Authentication.Requests;
using web.Interface;
using web.Interface.Components;
using web.Services.Authentication;

namespace web.Endpoints.Authentication;

public static class AuthenticationEndpoints
{
    private static IResult Auth<T>(HttpContext context) where T : IComponent
    {
        if (context.User.Identity is not { IsAuthenticated: true }) return new RazorHxResult<T>();

        context.Response.Headers.Append("HX-Redirect", "/");
        return Results.Redirect("/");
    }

    public static IEndpointRouteBuilder MapAuthenticaton(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/login", Auth<Login>).ExcludeFromDescription();
        builder.MapGet("/register", Auth<Register>).ExcludeFromDescription();

        builder.MapPost("/login",
                async ([FromForm] LoginRequest request, HttpContext context, UserManagementService userManagementService) =>
                {
                    if (context.User.Identity is { IsAuthenticated: true })
                    {
                        context.Response.Headers.Append("HX-Redirect", "/");
                        return Results.Ok();
                    }

                    var user = await userManagementService.Exists(request.Username);

                    if (user == null)
                        return new RazorHxResult<Login>(new
                        {
                            Username = request.Username,
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username or password is incorrect.",
                        });

                    var passwordMatch = UserManagementService.PasswordMatch(request.Password, user.PasswordHash);

                    if (!passwordMatch)
                        return new RazorHxResult<Login>(new
                        {
                            Username = request.Username,
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username or password is incorrect.",
                        });

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, user.Username),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties();

                    await context.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    context.Response.Headers.Append("HX-Redirect", "/");
                    return Results.Ok();
                }).ExcludeFromDescription()
            .DisableAntiforgery();

        builder.MapPost("/register",
                async ([FromForm] RegisterRequest request, HttpContext context, UserManagementService userManagementService) =>
                {
                    if (context.User.Identity is { IsAuthenticated: true })
                    {
                        context.Response.Headers.Append("HX-Redirect", "/");
                        return Results.Ok();
                    }

                    if (string.IsNullOrWhiteSpace(request.Username))
                        return new RazorHxResult<Register>(new
                        {
                            Username = request.Username,
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username is required.",
                        });

                    if (string.IsNullOrWhiteSpace(request.Password))
                        return new RazorHxResult<Register>(new
                        {
                            Username = request.Username,
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The password is required.",
                        });

                    if (request.Password != request.ConfirmPassword)
                        return new RazorHxResult<Register>(new
                        {
                            Username = request.Username,
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The passwords do not match.",
                        });

                    var user = await userManagementService.Exists(request.Username);

                    if (user != null)
                        return new RazorHxResult<Register>(new
                        {
                            Username = request.Username,
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username is already taken.",
                        });

                    _ = await userManagementService.Create(request.Username, request.Password);

                    context.Response.Headers.Append("HX-Redirect", "/");

                    return Results.Ok();
                }).ExcludeFromDescription()
            .DisableAntiforgery();

        builder.MapGet("/logout", async (HttpContext context) =>
            {
                await context.SignOutAsync();

                context.Response.Headers.Append("HX-Redirect", "/");
                return Results.Ok();
            }).ExcludeFromDescription()
            .RequireAuthorization();

        return builder;
    }
}