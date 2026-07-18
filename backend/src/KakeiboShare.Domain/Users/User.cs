using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Users;

/// <summary>
/// ユーザー（集約ルート）。Emailは一意（DB制約）。パスワードは必ずハッシュで保持する。
/// </summary>
public sealed class User
{
    public Guid Id { get; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string PasswordHash { get; private set; }

    private User(Guid id, string email, string name, string passwordHash)
    {
        Id = id;
        Email = email;
        Name = name;
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// メール・名前・ハッシュからユーザーを新規作成する。
    /// </summary>
    public static User Create(string email, string name, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new DomainException("メールアドレスは必須です");
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("名前は必須です");
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new DomainException("パスワードハッシュは必須です");

        return new User(Guid.NewGuid(), email.Trim(), name.Trim(), passwordHash);
    }

    /// <summary>
    /// 永続化層からの再構成用（Phase 3）。
    /// </summary>
    internal static User Reconstitute(Guid id, string email, string name, string passwordHash) =>
        new(id, email, name, passwordHash);
}
