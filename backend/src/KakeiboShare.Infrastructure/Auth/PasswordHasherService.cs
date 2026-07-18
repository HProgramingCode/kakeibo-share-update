using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace KakeiboShare.Infrastructure.Auth;

/// <summary>
/// ASP.NET Core 標準の PasswordHasher を使ったパスワードハッシュ実装。
/// </summary>
internal sealed class PasswordHasherService : IPasswordHasher
{
    private static readonly PasswordHasher<User> Hasher = new();
    private static readonly User HashContext = User.Reconstitute(
        Guid.Empty, "hash@local", "hash", "hash");

    /// <inheritdoc />
    public string HashPassword(string password) => Hasher.HashPassword(HashContext, password);

    /// <inheritdoc />
    public bool VerifyPassword(string hashedPassword, string password) =>
        Hasher.VerifyHashedPassword(HashContext, hashedPassword, password)
            != PasswordVerificationResult.Failed;
}
