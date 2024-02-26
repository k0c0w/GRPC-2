using Microsoft.AspNetCore.Server.Kestrel.Core;
using WeatherForecast.Services;
using WeatherForecast.Services.Abstractions;
using WeatherForecast.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherDataRetriever, WeatherDataRetriever>();
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel((options) =>
{
        options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGrpcService<WeatherForecastStreamerService>();

app.Run();

