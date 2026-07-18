using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Domain.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KakeiboShare.Infrastructure.Auth;

/// <summary>
/// JWT アクセストークンを発行する。Claims の sub に userId を設定する。
/// </summary>
internal sealed class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
    /// <inheritdoc />
    public string GenerateToken(User user)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.Key))
            throw new InvalidOperationException("Jwt:Key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
        };

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(settings.ExpiresDays),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
