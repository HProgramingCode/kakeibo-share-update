using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Application.Features.Expenses.ListExpenses;

/// <summary>
/// 支出一覧の1件。
/// </summary>
public sealed record ListExpensesItemResponse(
    Guid Id,
    int Amount,
    Guid PaidByUserId,
    Category Category,
    DateOnly ExpenseDate,
    bool Settled);

/// <summary>
/// グループの支出一覧を取得する。
/// </summary>
public sealed class ListExpensesHandler(
    IExpenseRepository expenses,
    IGroupMembershipChecker membershipChecker)
{
    /// <summary>
    /// 認可後に支出一覧を返す。
    /// </summary>
    public async Task<IReadOnlyList<ListExpensesItemResponse>> HandleAsync(
        Guid groupId,
        bool? settled,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var list = await expenses.ListSummariesByGroupAsync(groupId, settled, cancellationToken);
        return list
            .Select(e => new ListExpensesItemResponse(
                e.Id,
                e.Amount,
                e.PaidByUserId,
                e.Category,
                e.ExpenseDate,
                e.Settled))
            .ToList();
    }
}
