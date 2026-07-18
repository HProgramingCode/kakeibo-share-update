using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// Expenses テーブルに対応する永続化モデル。
/// グループ内の支出記録と精算紐付けを保持する。
/// </summary>
public sealed class ExpenseEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public Guid PaidByUserId { get; set; }
    public int Amount { get; set; }
    public Category Category { get; set; }
    public string Description { get; set; } = "";
    public DateOnly ExpenseDate { get; set; }
    public SplitType SplitType { get; set; }
    public Guid? SettlementId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public GroupEntity Group { get; set; } = null!;
    public UserEntity PaidByUser { get; set; } = null!;
    public SettlementEntity? Settlement { get; set; }
    public ICollection<ExpenseShareEntity> Shares { get; set; } = [];
}
