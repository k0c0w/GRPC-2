namespace WeatherForecast.Services.Abstractions;

public interface IWeatherDataRetriever
{
    public Task<float> RetriveWeatherAsync(DateTime dateTime);
}
