using Grpc.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Protobufs.Public;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WeatherForecast.Services;

public class AuthorizationService : Protobufs.Public.PublicService.PublicServiceBase
{
    private readonly JwtInfo _jwtInfo;

    public AuthorizationService(IOptions<JwtInfo> jwtOptions)
    {
        _jwtInfo = jwtOptions.Value;
    }

    public override Task<JwtResponse> GetJwt(JwtRequest request, ServerCallContext context)
    {
        var token = CreateToken();

        return Task.FromResult(new JwtResponse() { Token = token });
    }

    private string CreateToken()
    {
        var someClaims = new Claim[]{
                new Claim(JwtRegisteredClaimNames.NameId,Guid.NewGuid().ToString())
            };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtInfo.Key));
        var token = new JwtSecurityToken(
            issuer: _jwtInfo.Issuer,
            audience: _jwtInfo.Audience,
            claims: someClaims,
            expires: DateTime.Now.AddMinutes(3),
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
