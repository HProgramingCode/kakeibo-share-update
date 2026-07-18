using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Features.Settlements.CompleteSettlement;
using KakeiboShare.Application.Tests.Fakes;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;
using KakeiboShare.Domain.Groups;

namespace KakeiboShare.Application.Tests.Features.Settlements;

public class CompleteSettlementHandlerTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    [Fact]
    public async Task HandleAsync_未精算支出あり_精算が保存される()
    {
        var group = Group.Create("テスト", UserA);
        group.AddMember(UserB);

        var expense = Expense.Create(
            group.Id,
            UserA,
            Category.Food,
            SplitType.Equal,
            1000,
            [new ExpenseShare(UserA, 500), new ExpenseShare(UserB, 500)]);

        var expenses = new FakeExpenseRepository();
        expenses.Seed(new ExpenseDetail(expense, "ランチ", new DateOnly(2026, 7, 1), new Dictionary<Guid, int?>()));

        var settlements = new FakeSettlementRepository();
        var membership = new FakeMembershipChecker();
        membership.Allowed.Add((group.Id, UserA));
        var uow = new FakeUnitOfWork();

        var handler = new CompleteSettlementHandler(expenses, settlements, membership, uow);
        var response = await handler.HandleAsync(group.Id, UserA);

        Assert.NotNull(settlements.LastAdded);
        Assert.Equal(response.SettlementId, settlements.LastAdded!.Id);
        Assert.Equal(1, uow.SaveCount);
        Assert.True(expense.IsSettled);
    }

    [Fact]
    public async Task HandleAsync_未精算支出なし_例外()
    {
        var group = Group.Create("テスト", UserA);
        var handler = new CompleteSettlementHandler(
            new FakeExpenseRepository(),
            new FakeSettlementRepository(),
            new FakeMembershipChecker { Allowed = { (group.Id, UserA) } },
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(group.Id, UserA));
    }
}
