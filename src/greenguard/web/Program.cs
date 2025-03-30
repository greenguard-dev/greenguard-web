using Marten;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using RazorHx.Builder;
using RazorHx.DependencyInjection;
using RazorHx.Htmx.HttpContextFeatures;
using Weasel.Core;
using web.Endpoints;
using web.Endpoints.Hub;
using web.Endpoints.Plant;
using web.Services.Authentication;
using web.Services.Hub;
using web.Services.Version;
using web.Store;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorHxComponents(options => { options.RootComponent = typeof(web.Interface.Index); });

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("Marten")!);
builder.Services.AddMarten(options =>
{
    options.UseSystemTextJsonForSerialization();

    options.Schema.For<User>();
    options.Schema.For<Hub>();

    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
}).UseLightweightSessions().UseNpgsqlDataSource();

builder.Services.AddScoped<UserManagementService>();
builder.Services.AddSingleton<IVersionInfo>(new VersionInfo
    { Version = ThisAssembly.AssemblyFileVersion, InformationalVersion = ThisAssembly.AssemblyInformationalVersion });
builder.Services.AddScoped<IHubService, HubService>();
builder.Services.AddHttpClient<IHubClient, HubClient>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "greenguard-session";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
        options.Events.OnRedirectToLogin = context =>
        {
            var htmxRequestFeature = context.HttpContext.Features.Get<IHtmxRequestFeature>();
            if (htmxRequestFeature?.CurrentRequest.Request == true)
            {
                // Force htmx to refresh the page
                context.Response.Headers["HX-Refresh"] = "true";
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }

            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context => Task.CompletedTask;
    });

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseRazorHxComponents();

app.MapHubEndpoints();
app.MapApiHubEndpoints();
app.MapPlantEndpoints();
app.MapPlantApiEndpoints();
app.MapDefault();
app.MapAuthenticaton();

app.Run();