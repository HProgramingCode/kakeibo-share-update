using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Expenses;

/// <summary>支出（集約ルート）。負担合計＝金額。未精算のときのみ編集・削除できる。</summary>
public sealed class Expense
{
    public Guid Id { get; }
    public Guid GroupId { get; }
    public Guid PayerId { get; }
    public Category Category { get; }
    public SplitType SplitType { get; }
    public int Amount { get; }
    public IReadOnlyList<ExpenseShare> Shares { get; }
    public Guid? SettlementId { get; private set; }

    public bool IsSettled => SettlementId.HasValue;

    private Expense(Guid id, Guid groupId, Guid payerId, Category category, SplitType splitType,
        int amount, IReadOnlyList<ExpenseShare> shares)
    {
        Id = id;
        GroupId = groupId;
        PayerId = payerId;
        Category = category;
        SplitType = splitType;
        Amount = amount;
        Shares = shares;
    }

    public static Expense Create(Guid groupId, Guid payerId, Category category, SplitType splitType,
        int amount, IReadOnlyList<ExpenseShare> shares)
    {
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (shares.Count == 0) throw new DomainException("負担者が1人以上必要です");
        if (shares.Sum(s => s.Amount) != amount) throw new DomainException("負担合計は金額と一致する必要があります");
        if (shares.All(s => s.UserId != payerId)) throw new DomainException("支払者は負担者に含まれる必要があります");

        return new Expense(Guid.NewGuid(), groupId, payerId, category, splitType, amount, shares);
    }

    /// <summary>精算に紐付けてロックする。未精算のときのみ可能。</summary>
    public void AttachToSettlement(Guid settlementId)
    {
        if (IsSettled) throw new DomainException("既に精算確定済みの支出です");
        SettlementId = settlementId;
    }

    /// <summary>編集・削除の前提を満たすか検証する。精算確定済みなら例外。</summary>
    public void EnsureEditable()
    {
        if (IsSettled) throw new DomainException("精算確定済みの支出は編集・削除できません");
    }
}
