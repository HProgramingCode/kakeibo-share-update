using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Expenses;

/// <summary>
/// 支出（集約ルート）。負担合計＝金額。未精算のときのみ編集・削除できる。
/// </summary>
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
        int amount, IReadOnlyList<ExpenseShare> shares, Guid? settlementId = null)
    {
        Id = id;
        GroupId = groupId;
        PayerId = payerId;
        Category = category;
        SplitType = splitType;
        Amount = amount;
        Shares = shares;
        SettlementId = settlementId;
    }

    /// <summary>
    /// 支払者・金額・割り勘結果から支出を新規作成する。
    /// 負担合計は金額と一致すること。
    /// </summary>
    public static Expense Create(Guid groupId, Guid payerId, Category category, SplitType splitType,
        int amount, IReadOnlyList<ExpenseShare> shares)
    {
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (shares.Count == 0) throw new DomainException("負担者が1人以上必要です");
        if (shares.Sum(s => s.Amount) != amount) throw new DomainException("負担合計は金額と一致する必要があります");
        if (shares.All(s => s.UserId != payerId)) throw new DomainException("支払者は負担者に含まれる必要があります");

        return new Expense(Guid.NewGuid(), groupId, payerId, category, splitType, amount, shares);
    }

    /// <summary>
    /// 永続化層からの再構成用（Phase 3）。
    /// </summary>
    internal static Expense Reconstitute(Guid id, Guid groupId, Guid payerId, Category category,
        SplitType splitType, int amount, IReadOnlyList<ExpenseShare> shares, Guid? settlementId) =>
        new(id, groupId, payerId, category, splitType, amount, shares, settlementId);

    /// <summary>
    /// 精算に紐付けてロックする。未精算のときのみ可能。
    /// </summary>
    public void AttachToSettlement(Guid settlementId)
    {
        if (IsSettled) throw new DomainException("既に精算確定済みの支出です");
        SettlementId = settlementId;
    }

    /// <summary>
    /// 編集・削除の前提を満たすか検証する。精算確定済みなら例外。
    /// </summary>
    public void EnsureEditable()
    {
        if (IsSettled) throw new DomainException("精算確定済みの支出は編集・削除できません");
    }

    /// <summary>
    /// 未精算支出の内容を差し替えた新インスタンスを返す（ID・GroupId・SettlementIdは維持）。
    /// </summary>
    public Expense WithUpdatedContent(Guid payerId, Category category, SplitType splitType,
        int amount, IReadOnlyList<ExpenseShare> shares)
    {
        EnsureEditable();
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (shares.Count == 0) throw new DomainException("負担者が1人以上必要です");
        if (shares.Sum(s => s.Amount) != amount) throw new DomainException("負担合計は金額と一致する必要があります");
        if (shares.All(s => s.UserId != payerId)) throw new DomainException("支払者は負担者に含まれる必要があります");

        return new Expense(Id, GroupId, payerId, category, splitType, amount, shares, SettlementId);
    }
}
