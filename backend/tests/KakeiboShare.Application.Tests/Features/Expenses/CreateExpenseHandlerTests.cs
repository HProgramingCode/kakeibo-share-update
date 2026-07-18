using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Features.Expenses.CreateExpense;
using KakeiboShare.Application.Tests.Fakes;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;
using KakeiboShare.Domain.Groups;

namespace KakeiboShare.Application.Tests.Features.Expenses;

public class CreateExpenseHandlerTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    [Fact]
    public async Task HandleAsync_EqualSplit_負担合計が金額と一致する()
    {
        var group = Group.Create("テスト", UserA);
        group.AddMember(UserB);

        var groups = new FakeGroupRepository();
        groups.Seed(group);

        var expenses = new FakeExpenseRepository();
        var membership = new FakeMembershipChecker();
        membership.Allowed.Add((group.Id, UserA));
        var uow = new FakeUnitOfWork();

        var handler = new CreateExpenseHandler(groups, expenses, membership, uow);
        var response = await handler.HandleAsync(
            group.Id,
            new CreateExpenseRequest(
                1000,
                UserA,
                Category.Food,
                "ランチ",
                new DateOnly(2026, 7, 1),
                SplitType.Equal,
                [new ShareInput(UserA, null), new ShareInput(UserB, null)]),
            UserA);

        Assert.Equal(1000, response.Shares.Sum(s => s.ShareAmount));
        Assert.Equal(1, uow.SaveCount);
        Assert.Single(expenses.AllDetails);
    }

    [Fact]
    public async Task HandleAsync_Ratio合計が100でない_例外()
    {
        var group = Group.Create("テスト", UserA);
        group.AddMember(UserB);

        var groups = new FakeGroupRepository();
        groups.Seed(group);

        var handler = new CreateExpenseHandler(
            groups,
            new FakeExpenseRepository(),
            new FakeMembershipChecker { Allowed = { (group.Id, UserA) } },
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<DomainException>(() => handler.HandleAsync(
            group.Id,
            new CreateExpenseRequest(
                1000,
                UserA,
                Category.Food,
                "ランチ",
                new DateOnly(2026, 7, 1),
                SplitType.Ratio,
                [new ShareInput(UserA, 40), new ShareInput(UserB, 40)]),
            UserA));
    }

    [Fact]
    public async Task HandleAsync_非メンバー_ForbiddenException()
    {
        var group = Group.Create("テスト", UserA);

        var groups = new FakeGroupRepository();
        groups.Seed(group);

        var handler = new CreateExpenseHandler(
            groups,
            new FakeExpenseRepository(),
            new FakeMembershipChecker(),
            new FakeUnitOfWork());

        await Assert.ThrowsAsync<ForbiddenException>(() => handler.HandleAsync(
            group.Id,
            new CreateExpenseRequest(
                1000,
                UserA,
                Category.Food,
                "ランチ",
                new DateOnly(2026, 7, 1),
                SplitType.Equal,
                [new ShareInput(UserA, null)]),
            UserB));
    }
}
