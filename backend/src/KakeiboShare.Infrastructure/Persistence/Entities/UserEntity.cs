namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// Users テーブルに対応する永続化モデル。
/// 認証用メール・パスワードハッシュとグループ所属を保持する。
/// </summary>
public sealed class UserEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<GroupMemberEntity> GroupMemberships { get; set; } = [];
}
