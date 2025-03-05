using RazorHx.Builder;
using RazorHx.DependencyInjection;
using web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorHxComponents(options => { options.RootComponent = typeof(web.Interface.Index); });

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
app.UseRazorHxComponents();

app.MapDefault();

app.Run();