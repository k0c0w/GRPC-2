using Grpc.Net.Client;
using Protobufs; 

using var channel = GrpcChannel.ForAddress("http://localhost:5275");

var client = new Protobufs.WeatherForecastStreamer.WeatherForecastStreamerClient(channel);

var serverData = client.WeatherForecastStream(new ForecastRequest());

var responseStream = serverData.ResponseStream;
while (await responseStream.MoveNext(new CancellationToken()))
{
    ForecastResponse response = responseStream.Current;
    var time = response.Timestamp.ToDateTime();
    Console.WriteLine($"{DateTime.Now} погода на {time} = {response.Temperature}`C");
}