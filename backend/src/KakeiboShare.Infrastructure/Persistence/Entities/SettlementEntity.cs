using KakeiboShare.Domain.Settlements;

namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// Settlements テーブルに対応する永続化モデル。
/// グループの精算確定と送金一覧を保持する。
/// </summary>
public sealed class SettlementEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public SettlementStatus Status { get; set; }
    public DateTimeOffset SettledAt { get; set; }

    public GroupEntity Group { get; set; } = null!;
    public ICollection<SettlementTransferEntity> Transfers { get; set; } = [];
    public ICollection<ExpenseEntity> Expenses { get; set; } = [];
}
