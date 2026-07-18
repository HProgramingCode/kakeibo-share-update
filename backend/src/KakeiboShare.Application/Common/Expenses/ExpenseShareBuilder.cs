using KakeiboShare.Application.Common.Models;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Application.Common.Expenses;

/// <summary>
/// 割り勘計算と Expense 集約の組み立てを行う共通ヘルパー。
/// </summary>
public static class ExpenseShareBuilder
{
    /// <summary>
    /// 割り勘方式に応じて負担額を計算し、Expense と shareRatios を返す。
    /// </summary>
    public static (Expense Expense, IReadOnlyDictionary<Guid, int?> ShareRatios) Build(
        Guid groupId,
        Guid payerId,
        Category category,
        SplitType splitType,
        int amount,
        IReadOnlyList<ShareInput> shares,
        IReadOnlySet<Guid> groupMemberIds)
    {
        ValidateMembers(shares, groupMemberIds, payerId);

        var memberIds = shares.Select(s => s.UserId).ToList();
        var shareRatios = BuildShareRatios(splitType, shares);

        var amounts = splitType switch
        {
            SplitType.Equal => SplitCalculator.SplitEqual(amount, memberIds, payerId),
            SplitType.Ratio => SplitCalculator.SplitRatio(
                amount,
                shares.Select(s => (s.UserId, s.Ratio!.Value)).ToList(),
                payerId),
            _ => throw new DomainException("未対応の割り勘方式です"),
        };

        var expenseShares = amounts
            .Select(kv => new ExpenseShare(kv.Key, kv.Value))
            .ToList();

        var expense = Expense.Create(groupId, payerId, category, splitType, amount, expenseShares);
        return (expense, shareRatios);
    }

    /// <summary>
    /// 既存支出の内容を差し替えた Expense と shareRatios を返す。
    /// </summary>
    public static (Expense Expense, IReadOnlyDictionary<Guid, int?> ShareRatios) Rebuild(
        Expense existing,
        Guid payerId,
        Category category,
        SplitType splitType,
        int amount,
        IReadOnlyList<ShareInput> shares,
        IReadOnlySet<Guid> groupMemberIds)
    {
        ValidateMembers(shares, groupMemberIds, payerId);

        var memberIds = shares.Select(s => s.UserId).ToList();
        var shareRatios = BuildShareRatios(splitType, shares);

        var amounts = splitType switch
        {
            SplitType.Equal => SplitCalculator.SplitEqual(amount, memberIds, payerId),
            SplitType.Ratio => SplitCalculator.SplitRatio(
                amount,
                shares.Select(s => (s.UserId, s.Ratio!.Value)).ToList(),
                payerId),
            _ => throw new DomainException("未対応の割り勘方式です"),
        };

        var expenseShares = amounts
            .Select(kv => new ExpenseShare(kv.Key, kv.Value))
            .ToList();

        var expense = existing.WithUpdatedContent(payerId, category, splitType, amount, expenseShares);
        return (expense, shareRatios);
    }

    private static void ValidateMembers(IReadOnlyList<ShareInput> shares, IReadOnlySet<Guid> groupMemberIds, Guid payerId)
    {
        if (shares.Count == 0) throw new DomainException("負担者が1人以上必要です");
        if (!groupMemberIds.Contains(payerId)) throw new DomainException("支払者はグループメンバーである必要があります");

        foreach (var share in shares)
        {
            if (!groupMemberIds.Contains(share.UserId))
                throw new DomainException("対象メンバーはすべてグループに所属している必要があります");
        }
    }

    private static IReadOnlyDictionary<Guid, int?> BuildShareRatios(SplitType splitType, IReadOnlyList<ShareInput> shares)
    {
        if (splitType == SplitType.Equal)
            return shares.ToDictionary(s => s.UserId, _ => (int?)null);

        if (shares.Sum(s => s.Ratio ?? 0) != 100)
            throw new DomainException("割合の合計は100である必要があります");

        return shares.ToDictionary(s => s.UserId, s => s.Ratio);
    }
}
