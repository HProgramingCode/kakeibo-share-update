using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Application.Features.Auth;

namespace KakeiboShare.Application.Features.Auth.Login;

/// <summary>
/// ログインリクエスト。
/// </summary>
public sealed record LoginRequest(string Email, string Password);

/// <summary>
/// 資格情報を照合し JWT を発行する。
/// </summary>
public sealed class LoginHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService)
{
    /// <summary>
    /// メールとパスワードを検証し、成功時にトークンを返す。
    /// </summary>
    public async Task<AuthResponse> HandleAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null || !passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            throw new UnauthorizedException("メールアドレスまたはパスワードが正しくありません");

        var token = jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, new AuthUserResponse(user.Id, user.Name, user.Email));
    }
}
