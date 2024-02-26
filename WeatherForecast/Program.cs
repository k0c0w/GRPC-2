using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherForecast;
using WeatherForecast.Services;
using WeatherForecast.Services.Abstractions;
using WeatherForecast.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IWeatherDataRetriever, WeatherDataRetriever>();
builder.Services.AddGrpc();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWT").GetValue<string>("Key")!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<JwtInfo>(builder.Configuration.GetSection("JWT"));

builder.WebHost.ConfigureKestrel((options) =>
{
    options.ConfigureEndpointDefaults(lo => lo.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapGrpcService<WeatherForecastStreamerService>();
app.MapGrpcService<AuthorizationService>();
app.MapGrpcService<PrivateService>();

app.Run();

