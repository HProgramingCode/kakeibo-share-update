using KakeiboShare.Domain.Expenses;
using KakeiboShare.Domain.Settlements;

namespace KakeiboShare.Domain.Tests.Settlements;

public class NetBalancesTests
{
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();
    private static readonly Guid C = Guid.NewGuid();

    private static Expense Expense(Guid payer, int amount, params (Guid id, int amt)[] shares) =>
        KakeiboShare.Domain.Expenses.Expense.Create(
            Guid.NewGuid(), payer, Category.Food, SplitType.Equal, amount,
            shares.Select(s => new ExpenseShare(s.id, s.amt)).ToArray());

    [Fact]
    public void 立替から負担を引いた純額になる()
    {
        // A が1000円立替、負担 A334/B333/C333
        var expenses = new[] { Expense(A, 1000, (A, 334), (B, 333), (C, 333)) };

        var net = SettlementCalculator.NetBalances(expenses);

        Assert.Equal(666, net[A]);   // 1000立替 − 334負担
        Assert.Equal(-333, net[B]);
        Assert.Equal(-333, net[C]);
    }

    [Fact]
    public void 複数支出でも純額の総和は0()
    {
        var expenses = new[]
        {
            Expense(A, 900, (A, 300), (B, 300), (C, 300)),
            Expense(B, 600, (A, 300), (B, 300)),
        };

        var net = SettlementCalculator.NetBalances(expenses);

        Assert.Equal(0, net.Values.Sum());
    }

    [Fact]
    public void 支出がなければ空()
    {
        var net = SettlementCalculator.NetBalances(Array.Empty<Expense>());
        Assert.Empty(net);
    }
}
