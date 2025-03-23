using Marten;
using Microsoft.AspNetCore.Authentication.Cookies;
using RazorHx.Builder;
using RazorHx.DependencyInjection;
using Weasel.Core;
using web.Endpoints;
using web.Services.Authentication;
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

    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }
}).UseLightweightSessions().UseNpgsqlDataSource();

builder.Services.AddScoped<UserManagementService>();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "greenguard-session";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.UseRazorHxComponents();

app.MapDefault();
app.MapAuthenticaton();

app.Run();