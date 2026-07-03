namespace KakeiboShare.Domain.Common;

/// <summary>
/// 円・整数の金額。純額計算で負数も許容する。
/// </summary>
public readonly record struct Money(int Yen)
{
    public static Money operator +(Money a, Money b) => new(a.Yen + b.Yen);
    public static Money operator -(Money a, Money b) => new(a.Yen - b.Yen);
}
