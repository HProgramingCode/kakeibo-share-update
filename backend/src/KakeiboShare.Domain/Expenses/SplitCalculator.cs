using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Expenses;

/// <summary>
/// 割り勘計算（純粋関数）。負担合計は必ず金額に一致し、端数・丸め差は支払者が吸収する。
/// </summary>
public static class SplitCalculator
{
    /// <summary>
    /// 割合配分で各メンバーの割合合計が満たすべき値（%）。
    /// </summary>
    private const int TotalRatioPercent = 100;

    /// <summary>
    /// 金額を対象メンバーで均等割りする。端数は支払者が吸収する。
    /// </summary>
    public static IReadOnlyDictionary<Guid, int> SplitEqual(int amount, IReadOnlyList<Guid> memberIds, Guid payerId)
    {
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (memberIds.Count == 0) throw new DomainException("対象メンバーが1人以上必要です");
        if (!memberIds.Contains(payerId)) throw new DomainException("支払者は対象メンバーに含まれる必要があります");

        var memberCount = memberIds.Count;
        var sharePerMember = amount / memberCount;                 // floor（amount>0, memberCount>0）
        var remainder = amount - sharePerMember * memberCount;     // 0 <= remainder < memberCount

        var shares = memberIds.ToDictionary(memberId => memberId, _ => sharePerMember);
        shares[payerId] += remainder;               // 端数は支払者が吸収
        return shares;
    }

    /// <summary>
    /// 金額を割合（合計100）で配分する。丸め差は支払者が吸収する。
    /// </summary>
    public static IReadOnlyDictionary<Guid, int> SplitRatio(int amount, IReadOnlyList<(Guid id, int ratio)> ratios, Guid payerId)
    {
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (ratios.Count == 0) throw new DomainException("対象メンバーが1人以上必要です");
        if (ratios.Sum(memberRatio => memberRatio.ratio) != TotalRatioPercent) throw new DomainException("割合の合計は100である必要があります");
        if (!ratios.Any(memberRatio => memberRatio.id == payerId)) throw new DomainException("支払者は対象メンバーに含まれる必要があります");

        // 各自の負担 = round(金額 × 割合 / TotalRatioPercent)。round は四捨五入（.5 は0から離れる方向）。
        var shares = ratios.ToDictionary(
            memberRatio => memberRatio.id,
            memberRatio => (int)Math.Round((decimal)amount * memberRatio.ratio / TotalRatioPercent, MidpointRounding.AwayFromZero));

        var roundingDifference = amount - shares.Values.Sum();    // 丸め差（正負あり）
        shares[payerId] += roundingDifference;                     // 差額は支払者が吸収
        return shares;
    }
}
