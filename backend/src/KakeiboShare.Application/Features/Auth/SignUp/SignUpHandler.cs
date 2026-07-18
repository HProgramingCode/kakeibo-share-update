using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Application.Features.Auth;
using KakeiboShare.Domain.Users;

namespace KakeiboShare.Application.Features.Auth.SignUp;

/// <summary>
/// ユーザー登録リクエスト。
/// </summary>
public sealed record SignUpRequest(string Email, string Password, string Name);

/// <summary>
/// 新規ユーザーを登録し JWT を発行する。
/// </summary>
public sealed class SignUpHandler(
    IUserRepository users,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// メール重複を検証し、ユーザーを作成してトークンを返す。
    /// </summary>
    public async Task<AuthResponse> HandleAsync(SignUpRequest request, CancellationToken cancellationToken = default)
    {
        if (await users.ExistsByEmailAsync(request.Email, cancellationToken))
            throw new ConflictException("このメールアドレスは既に登録されています");

        var passwordHash = passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Email, request.Name, passwordHash);

        await users.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var token = jwtTokenService.GenerateToken(user);
        return new AuthResponse(token, new AuthUserResponse(user.Id, user.Name, user.Email));
    }
}
