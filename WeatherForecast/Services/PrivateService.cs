using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Protobufs.Private;

namespace WeatherForecast.Services;

[Authorize]
public class PrivateService : Protobufs.Private.PrivateService.PrivateServiceBase
{
    public override Task<Response> GetPrivateInfo(Request request, ServerCallContext context)
    {
        return Task.FromResult(new Response() { Message = "Secret info" });
    }
}
