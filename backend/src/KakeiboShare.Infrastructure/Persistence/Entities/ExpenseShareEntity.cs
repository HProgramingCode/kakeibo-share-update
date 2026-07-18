namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// ExpenseShares テーブルに対応する永続化モデル。
/// 支出のメンバー別分担額を保持する。
/// </summary>
public sealed class ExpenseShareEntity
{
    public Guid Id { get; set; }
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    public int? ShareRatio { get; set; }
    public int ShareAmount { get; set; }

    public ExpenseEntity Expense { get; set; } = null!;
    public UserEntity User { get; set; } = null!;
}
