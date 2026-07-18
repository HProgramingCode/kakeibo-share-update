using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Application.Features.Expenses.GetExpense;

/// <summary>
/// 支出詳細の負担1件。
/// </summary>
public sealed record GetExpenseShareResponse(Guid UserId, int? Ratio, int ShareAmount);

/// <summary>
/// 支出詳細レスポンス。
/// </summary>
public sealed record GetExpenseResponse(
    Guid Id,
    Guid GroupId,
    int Amount,
    Guid PaidByUserId,
    Category Category,
    string Description,
    DateOnly ExpenseDate,
    SplitType SplitType,
    bool Settled,
    IReadOnlyList<GetExpenseShareResponse> Shares);

/// <summary>
/// 支出詳細を取得する。
/// </summary>
public sealed class GetExpenseHandler(
    IExpenseRepository expenses,
    IGroupMembershipChecker membershipChecker)
{
    /// <summary>
    /// 認可後に支出詳細を返す。
    /// </summary>
    public async Task<GetExpenseResponse> HandleAsync(
        Guid expenseId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberOfExpenseAsync(expenseId, currentUserId, cancellationToken);

        var detail = await expenses.GetDetailAsync(expenseId, cancellationToken)
            ?? throw new NotFoundException("支出が見つかりません");

        var expense = detail.Expense;
        return new GetExpenseResponse(
            expense.Id,
            expense.GroupId,
            expense.Amount,
            expense.PayerId,
            expense.Category,
            detail.Description,
            detail.ExpenseDate,
            expense.SplitType,
            expense.IsSettled,
            expense.Shares
                .Select(s => new GetExpenseShareResponse(
                    s.UserId,
                    detail.ShareRatios.GetValueOrDefault(s.UserId),
                    s.Amount))
                .ToList());
    }
}
