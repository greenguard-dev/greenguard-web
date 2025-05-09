using Marten;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using MQTTnet.AspNetCore;
using RazorHx.Builder;
using RazorHx.DependencyInjection;
using RazorHx.Htmx.HttpContextFeatures;
using Scalar.AspNetCore;
using Weasel.Core;
using web.Endpoints;
using web.Endpoints.Authentication;
using web.Endpoints.Hub;
using web.Endpoints.Plant;
using web.Services.Authentication;
using web.Services.Hub;
using web.Services.Mqtt;
using web.Services.Plant;
using web.Services.Version;
using web.Store;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(so =>
{
    so.ListenAnyIP(8080);
    so.ListenAnyIP(1883, l => l.UseMqtt());
});

builder.Services.AddHostedMqttServer(
    optionsBuilder => { optionsBuilder.WithoutDefaultEndpoint(); });

builder.Services.AddMqttConnectionHandler();

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

builder.Services.AddOpenApi("greenguard API");

builder.Services.AddRazorHxComponents(options => { options.RootComponent = typeof(web.Interface.Index); });

builder.Services.AddHttpContextAccessor();

builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("Marten")!);

builder.Services.AddMarten(options =>
    {
        options.UseSystemTextJsonForSerialization();

        options.Schema.For<User>();
        options.Schema.For<Hub>();
        options.Schema.For<Plant>()
            .AddSubClass<SensorlessPlant>()
            .AddSubClass<SensorPlant>();
        options.Schema.For<PlantMeasurement>()
            .AddSubClass<ManuelMeasurement>()
            .AddSubClass<SensorMeasurement>();

        if (builder.Environment.IsDevelopment())
        {
            options.AutoCreateSchemaObjects = AutoCreate.All;
        }
    }).UseLightweightSessions()
    .UseNpgsqlDataSource();

builder.Services.AddSingleton<IVersionInfo>(new VersionInfo
    { Version = ThisAssembly.AssemblyFileVersion, InformationalVersion = ThisAssembly.AssemblyInformationalVersion });

builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<IHubService, HubService>();
builder.Services.AddHttpClient<IHubClient, HubClient>();
builder.Services.AddScoped<IPlantService, PlantService>();
builder.Services.AddHostedService<MqttHubHostedService>();

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => { options.AddDocument("greenguard API", "greenguard API"); });
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