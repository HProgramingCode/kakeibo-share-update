using KakeiboShare.Domain.Users;

namespace KakeiboShare.Application.Common.Auth;

/// <summary>
/// 認証済みユーザー向け JWT アクセストークンを発行する。
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// ユーザー ID（sub）を含む JWT を生成する。
    /// </summary>
    string GenerateToken(User user);
}
