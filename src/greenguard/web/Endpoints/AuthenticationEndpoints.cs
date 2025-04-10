using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using RazorHx.Results;
using web.Interface;
using web.Interface.Components;
using web.Services.Authentication;

namespace web.Endpoints;

public static class AuthenticationEndpoints
{
    private static IResult Auth<T>(HttpContext context) where T : IComponent
    {
        if (context.User.Identity is { IsAuthenticated: true })
        {
            context.Response.Headers.Append("HX-Redirect", "/");
            return Results.Redirect("/");
        }

        return new RazorHxResult<T>();
    }

    public static IEndpointRouteBuilder MapAuthenticaton(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/login", Auth<Login>).ExcludeFromDescription();
        builder.MapGet("/register", Auth<Register>).ExcludeFromDescription();

        builder.MapPost("/login",
                async (HttpContext context, UserManagementService userManagementService) =>
                {
                    if (context.User.Identity is { IsAuthenticated: true })
                    {
                        context.Response.Headers.Append("HX-Redirect", "/");
                        return Results.Ok();
                    }

                    var form = context.Request.Form;
                    var username = form["username"];
                    var password = form["password"];

                    var user = await userManagementService.Exists(username);

                    if (user == null)
                        return new RazorHxResult<Login>(new
                        {
                            Username = username.ToString(),
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username or password is incorrect.",
                        });

                    var passwordMatch = UserManagementService.PasswordMatch(password, user.PasswordHash);

                    if (!passwordMatch)
                        return new RazorHxResult<Login>(new
                        {
                            Username = username.ToString(),
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
                async (HttpContext context, UserManagementService userManagementService) =>
                {
                    if (context.User.Identity is { IsAuthenticated: true })
                    {
                        context.Response.Headers.Append("HX-Redirect", "/");
                        return Results.Ok();
                    }

                    var form = context.Request.Form;
                    var username = form["username"];
                    var password = form["password"];
                    var repeat = form["repeat"];

                    if (string.IsNullOrWhiteSpace(username))
                        return new RazorHxResult<Register>(new
                        {
                            Username = username.ToString(),
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username is required.",
                        });
                    
                    if (string.IsNullOrWhiteSpace(password))
                        return new RazorHxResult<Register>(new
                        {
                            Username = username.ToString(),
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The password is required.",
                        });
                    
                    if (password != repeat)
                        return new RazorHxResult<Register>(new
                        {
                            Username = username.ToString(),
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The passwords do not match.",
                        });

                    var user = await userManagementService.Exists(username);

                    if (user != null)
                        return new RazorHxResult<Register>(new
                        {
                            Username = username.ToString(),
                        }).WithOutOfBand<ErrorSnackbar>(new
                        {
                            Message = "The username is already taken.",
                        });

                    _ = await userManagementService.Create(username, password);

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