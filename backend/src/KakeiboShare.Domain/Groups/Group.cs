using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Groups;

/// <summary>
/// グループ（集約ルート）。メンバーは(Group,User)で一意。招待コードは再発行で旧コードが無効化される。
/// </summary>
public sealed class Group
{
    private readonly List<Guid> _memberUserIds = [];

    public Guid Id { get; }
    public string Name { get; private set; }
    public InviteCode InviteCode { get; private set; }
    public IReadOnlyList<Guid> MemberUserIds => _memberUserIds;

    private Group(Guid id, string name, InviteCode inviteCode)
    {
        Id = id;
        Name = name;
        InviteCode = inviteCode;
    }

    public static Group Create(string name, Guid creatorId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("グループ名は必須です");

        var group = new Group(Guid.NewGuid(), name.Trim(), InviteCode.Generate());
        group._memberUserIds.Add(creatorId); // 作成者を最初のメンバーに
        return group;
    }

    /// <summary>
    /// メンバーを追加する。(Group,User)は一意なので二重参加は不可。
    /// </summary>
    public void AddMember(Guid userId)
    {
        if (_memberUserIds.Contains(userId)) throw new DomainException("既に参加済みのメンバーです");
        _memberUserIds.Add(userId);
    }

    /// <summary>
    /// 招待コードを再発行する。旧コードは置き換えられ無効化される。
    /// </summary>
    public void RegenerateInvite() => InviteCode = InviteCode.Generate();
}
