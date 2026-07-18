using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KakeiboShare.Api.Extensions;

/// <summary>
/// JWT Claims からユーザー ID を取り出す拡張。
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// 認証済みユーザーの ID（sub）を取得する。
    /// </summary>
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (value is null || !Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("ユーザー ID を取得できません");

        return userId;
    }
}
