using Weather.Api.Models;
using Weather.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
var appName = builder.Environment.ApplicationName;

//Development_Weather.Api_OpenWeatherMapApi__ApiKey

builder.Configuration.AddSecretsManager(configurator: o =>
{
    o.SecretFilter = entry => entry.Name.StartsWith($"{env}_{appName}");
    o.KeyGenerator = (_, s) => s.Replace($"{env}_{appName}_", string.Empty).Replace("__", ":");
    o.PollingInterval = TimeSpan.FromDays(30);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<OpenWeatherApiSettings>(builder.Configuration.GetSection(OpenWeatherApiSettings.Key));
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IWeatherService, WeatherService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
