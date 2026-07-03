using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Domain.Tests.Expenses;

public class SplitCalculatorRatioTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();
    private static readonly Guid C = Guid.NewGuid();
    private static readonly Guid D = Guid.NewGuid();

    [Fact]
    public void 割合どおりに配分する_端数なし()
    {
        var shares = SplitCalculator.SplitRatio(1000, new[] { (A, 30), (B, 30), (C, 40) }, payerId: A);

        Assert.Equal(300, shares[A]);
        Assert.Equal(300, shares[B]);
        Assert.Equal(400, shares[C]);
        Assert.Equal(1000, shares.Values.Sum());
    }

    [Fact]
    public void 丸め差プラスは支払者が吸収する()
    {
        // 10円を 33/33/34 → round(3.3)=3, 3, round(3.4)=3 → 合計9、差 +1 は支払者A
        var shares = SplitCalculator.SplitRatio(10, new[] { (A, 33), (B, 33), (C, 34) }, payerId: A);

        Assert.Equal(4, shares[A]);
        Assert.Equal(3, shares[B]);
        Assert.Equal(3, shares[C]);
        Assert.Equal(10, shares.Values.Sum());
    }

    [Fact]
    public void 丸め差マイナスも支払者が吸収する()
    {
        // 10円を 25/25/25/25 → round(2.5)=3(四捨五入) x4 = 12、差 -2 は支払者A
        var shares = SplitCalculator.SplitRatio(10, new[] { (A, 25), (B, 25), (C, 25), (D, 25) }, payerId: A);

        Assert.Equal(1, shares[A]);
        Assert.Equal(3, shares[B]);
        Assert.Equal(3, shares[C]);
        Assert.Equal(3, shares[D]);
        Assert.Equal(10, shares.Values.Sum());
    }

    [Fact]
    public void 負担合計は常に金額と一致する()
    {
        var shares = SplitCalculator.SplitRatio(9999, new[] { (A, 33), (B, 33), (C, 34) }, payerId: B);
        Assert.Equal(9999, shares.Values.Sum());
    }

    [Fact]
    public void 割合合計が100でないなら例外()
    {
        Assert.Throws<DomainException>(() => SplitCalculator.SplitRatio(1000, new[] { (A, 30), (B, 30), (C, 30) }, payerId: A));
    }

    [Fact]
    public void 金額が0以下なら例外()
    {
        Assert.Throws<DomainException>(() => SplitCalculator.SplitRatio(0, new[] { (A, 100) }, payerId: A));
    }

    [Fact]
    public void 支払者が対象に含まれないなら例外()
    {
        Assert.Throws<DomainException>(() => SplitCalculator.SplitRatio(1000, new[] { (A, 50), (B, 50) }, payerId: C));
    }
}
