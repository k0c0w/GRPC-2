using Grpc.Core;
using Grpc.Net.Client;
using Protobufs.Public;

using var channel = GrpcChannel.ForAddress("http://localhost:5275");

var client = new PublicService.PublicServiceClient(channel);

var jwt = await client.GetJwtAsync(new JwtRequest());
var token = jwt.Token;


var headers = new Metadata
{
    { "Authorization", $"Bearer {token}" }
};

var secretClient = new Protobufs.Private.PrivateService.PrivateServiceClient(channel);
var secret = await secretClient.GetPrivateInfoAsync(new Protobufs.Private.Request(), headers);
Console.WriteLine(secret.Message);
Console.ReadKey();