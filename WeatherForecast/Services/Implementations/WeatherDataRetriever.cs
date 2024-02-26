using System.Text.Json.Nodes;
using WeatherForecast.Services.Abstractions;

namespace WeatherForecast.Services.Implementations;

public class WeatherDataRetriever : IWeatherDataRetriever
{
    private const string API = "https://api.open-meteo.com/v1/forecast";

    private readonly HttpClient _client;

    public WeatherDataRetriever(IHttpClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient();
    }

    public async Task<float> RetriveWeatherAsync(DateTime dateTime)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, 
            $"{API}?latitude=52.52&longitude=13.41&hourly=temperature_2m&start_hour={dateTime.ToString("yyyy-MM-ddTHH:mm")}&end_hour={dateTime.AddHours(1).ToString("yyyy-MM-ddTHH:mm")}");

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Bad request");

        var responseJson = await response.Content.ReadAsStringAsync();

        var parseJson = JsonNode.Parse(responseJson);

        return parseJson["hourly"]["temperature_2m"][0].GetValue<float>();
    }
}
