namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// GroupMembers テーブルに対応する永続化モデル。
/// ユーザーとグループの所属関係を表す。
/// </summary>
public sealed class GroupMemberEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid UserId { get; set; }
    public DateTimeOffset JoinedAt { get; set; }

    public GroupEntity Group { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}
