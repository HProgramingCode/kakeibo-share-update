namespace KakeiboShare.Application.Common.Auth;

/// <summary>
/// パスワードのハッシュ化と検証。平文は永続化しない。
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// 平文パスワードをハッシュ化する。
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// ハッシュと平文が一致するか検証する。
    /// </summary>
    bool VerifyPassword(string hashedPassword, string password);
}
