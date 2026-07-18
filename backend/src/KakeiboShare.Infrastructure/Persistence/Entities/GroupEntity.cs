namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// Groups テーブルに対応する永続化モデル。
/// 共有グループの基本情報と招待コードを保持する。
/// </summary>
public sealed class GroupEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string InviteCode { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<GroupMemberEntity> Members { get; set; } = [];
    public ICollection<ExpenseEntity> Expenses { get; set; } = [];
    public ICollection<SettlementEntity> Settlements { get; set; } = [];
}
