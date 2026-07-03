using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Expenses;

/// <summary>
/// 割り勘計算（純粋関数）。負担合計は必ず金額に一致し、端数・丸め差は支払者が吸収する。
/// </summary>
public static class SplitCalculator
{
    public static IReadOnlyDictionary<Guid, int> SplitEqual(int amount, IReadOnlyList<Guid> memberIds, Guid payerId)
    {
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (memberIds.Count == 0) throw new DomainException("対象メンバーが1人以上必要です");
        if (!memberIds.Contains(payerId)) throw new DomainException("支払者は対象メンバーに含まれる必要があります");

        var n = memberIds.Count;
        var baseShare = amount / n;                 // floor（amount>0, n>0）
        var remainder = amount - baseShare * n;     // 0 <= remainder < n

        var shares = memberIds.ToDictionary(id => id, _ => baseShare);
        shares[payerId] += remainder;               // 端数は支払者が吸収
        return shares;
    }

    public static IReadOnlyDictionary<Guid, int> SplitRatio(int amount, IReadOnlyList<(Guid id, int ratio)> ratios, Guid payerId)
    {
        if (amount <= 0) throw new DomainException("金額は正である必要があります");
        if (ratios.Count == 0) throw new DomainException("対象メンバーが1人以上必要です");
        if (ratios.Sum(r => r.ratio) != 100) throw new DomainException("割合の合計は100である必要があります");
        if (!ratios.Any(r => r.id == payerId)) throw new DomainException("支払者は対象メンバーに含まれる必要があります");

        // 各自の負担 = round(金額 × 割合 / 100)。round は四捨五入（.5 は0から離れる方向）。
        var shares = ratios.ToDictionary(
            r => r.id,
            r => (int)Math.Round((decimal)amount * r.ratio / 100m, MidpointRounding.AwayFromZero));

        var diff = amount - shares.Values.Sum();    // 丸め差（正負あり）
        shares[payerId] += diff;                     // 差額は支払者が吸収
        return shares;
    }
}
