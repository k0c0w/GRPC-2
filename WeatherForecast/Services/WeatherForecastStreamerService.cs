using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Protobufs;
using System.Diagnostics;
using System.Globalization;
using WeatherForecast.Services.Abstractions;

namespace WeatherForecast.Services;

public class WeatherForecastStreamerService : Protobufs.WeatherForecastStreamer.WeatherForecastStreamerBase
{
    private readonly IWeatherDataRetriever _dataRetriever;
    private readonly ILogger<WeatherForecastStreamerService> _logger;

    public WeatherForecastStreamerService(IWeatherDataRetriever weatherDataRetriever, ILogger<WeatherForecastStreamerService> logger)
    {
        _dataRetriever = weatherDataRetriever;
        _logger = logger;
    }

    public override async Task WeatherForecastStream(ForecastRequest request, IServerStreamWriter<ForecastResponse> responseStream, ServerCallContext context)
    {
        var stopwatch = new Stopwatch();
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var twoHoursTimespan = TimeSpan.FromHours(2);
        float tempC;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            stopwatch.Start();

            try
            {
                var response = await _dataRetriever.RetriveWeatherAsync(startDate);
                tempC = response;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"error while retriving data for time: {startDate}");
                continue;
            }

            await responseStream.WriteAsync(new ForecastResponse()
            {
                Timestamp = startDate.ToTimestamp(),
                Temperature = tempC
            });

            stopwatch.Stop();

            if (stopwatch.Elapsed.Seconds > 1)
                continue;
            else
                await Task.Delay((int)(1000 - stopwatch.ElapsedMilliseconds));

            stopwatch.Reset();
            startDate += twoHoursTimespan;
        }
    }
}
