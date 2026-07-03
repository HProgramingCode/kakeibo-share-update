using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Domain.Tests.Expenses;

public class ExpenseTests
{
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid Payer = Guid.NewGuid();
    private static readonly Guid Other = Guid.NewGuid();

    private static Expense CreateValid() => Expense.Create(
        groupId: GroupId,
        payerId: Payer,
        category: Category.Food,
        splitType: SplitType.Equal,
        amount: 1000,
        shares: new[] { new ExpenseShare(Payer, 500), new ExpenseShare(Other, 500) });

    [Fact]
    public void 生成直後は未精算()
    {
        var expense = CreateValid();
        Assert.False(expense.IsSettled);
        Assert.Null(expense.SettlementId);
        Assert.Equal(1000, expense.Amount);
        Assert.Equal(2, expense.Shares.Count);
    }

    [Fact]
    public void 負担合計が金額と一致しないなら例外()
    {
        Assert.Throws<DomainException>(() => Expense.Create(
            GroupId, Payer, Category.Food, SplitType.Equal, amount: 1000,
            shares: new[] { new ExpenseShare(Payer, 500), new ExpenseShare(Other, 400) }));
    }

    [Fact]
    public void 金額が0以下なら例外()
    {
        Assert.Throws<DomainException>(() => Expense.Create(
            GroupId, Payer, Category.Food, SplitType.Equal, amount: 0,
            shares: new[] { new ExpenseShare(Payer, 0) }));
    }

    [Fact]
    public void 支払者が負担者に含まれないなら例外()
    {
        Assert.Throws<DomainException>(() => Expense.Create(
            GroupId, Payer, Category.Food, SplitType.Equal, amount: 1000,
            shares: new[] { new ExpenseShare(Other, 1000) }));
    }

    [Fact]
    public void 精算に紐付けるとロックされる()
    {
        var expense = CreateValid();
        var settlementId = Guid.NewGuid();

        expense.AttachToSettlement(settlementId);

        Assert.True(expense.IsSettled);
        Assert.Equal(settlementId, expense.SettlementId);
    }

    [Fact]
    public void 精算確定済みは編集不可()
    {
        var expense = CreateValid();
        expense.AttachToSettlement(Guid.NewGuid());

        Assert.Throws<DomainException>(() => expense.EnsureEditable());
    }

    [Fact]
    public void 未精算なら編集可能()
    {
        var expense = CreateValid();
        expense.EnsureEditable(); // 例外を投げない
    }

    [Fact]
    public void 既に精算済みなら再度の紐付けは例外()
    {
        var expense = CreateValid();
        expense.AttachToSettlement(Guid.NewGuid());

        Assert.Throws<DomainException>(() => expense.AttachToSettlement(Guid.NewGuid()));
    }
}
