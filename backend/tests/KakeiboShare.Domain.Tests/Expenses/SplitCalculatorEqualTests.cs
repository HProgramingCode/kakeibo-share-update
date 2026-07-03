using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Domain.Tests.Expenses;

public class SplitCalculatorEqualTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();
    private static readonly Guid C = Guid.NewGuid();

    [Fact]
    public void 割り切れない端数は支払者が吸収する_1000円を3人()
    {
        // payer = C
        var shares = SplitCalculator.SplitEqual(1000, new[] { A, B, C }, payerId: C);

        Assert.Equal(333, shares[A]);
        Assert.Equal(333, shares[B]);
        Assert.Equal(334, shares[C]); // 端数 +1 は支払者
        Assert.Equal(1000, shares.Values.Sum());
    }

    [Fact]
    public void 割り切れる場合は全員同額()
    {
        var shares = SplitCalculator.SplitEqual(900, new[] { A, B, C }, payerId: A);

        Assert.Equal(300, shares[A]);
        Assert.Equal(300, shares[B]);
        Assert.Equal(300, shares[C]);
        Assert.Equal(900, shares.Values.Sum());
    }

    [Fact]
    public void 対象が1人なら全額を負担する()
    {
        var shares = SplitCalculator.SplitEqual(500, new[] { A }, payerId: A);

        Assert.Equal(500, shares[A]);
    }

    [Fact]
    public void 負担合計は常に金額と一致する()
    {
        var shares = SplitCalculator.SplitEqual(1001, new[] { A, B, C }, payerId: B);
        Assert.Equal(1001, shares.Values.Sum());
    }

    [Fact]
    public void 金額が0以下なら例外()
    {
        Assert.Throws<DomainException>(() => SplitCalculator.SplitEqual(0, new[] { A }, payerId: A));
    }

    [Fact]
    public void 対象メンバーが空なら例外()
    {
        Assert.Throws<DomainException>(() => SplitCalculator.SplitEqual(100, Array.Empty<Guid>(), payerId: A));
    }

    [Fact]
    public void 支払者が対象メンバーに含まれないなら例外()
    {
        Assert.Throws<DomainException>(() => SplitCalculator.SplitEqual(100, new[] { A, B }, payerId: C));
    }
}
