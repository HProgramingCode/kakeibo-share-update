using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Expenses;
using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Application.Features.Expenses.CreateExpense;

/// <summary>
/// 支出登録リクエスト。
/// </summary>
public sealed record CreateExpenseRequest(
    int Amount,
    Guid PaidByUserId,
    Category Category,
    string Description,
    DateOnly ExpenseDate,
    SplitType SplitType,
    IReadOnlyList<ShareInput> Shares);

/// <summary>
/// 支出の負担1件。
/// </summary>
public sealed record ExpenseShareResponse(Guid UserId, int ShareAmount);

/// <summary>
/// 支出登録レスポンス。
/// </summary>
public sealed record CreateExpenseResponse(
    Guid Id,
    Guid GroupId,
    int Amount,
    Guid PaidByUserId,
    Category Category,
    string Description,
    DateOnly ExpenseDate,
    SplitType SplitType,
    bool Settled,
    IReadOnlyList<ExpenseShareResponse> Shares);

/// <summary>
/// 支出を登録し、割り勘結果を Shares として永続化する。
/// </summary>
public sealed class CreateExpenseHandler(
    IGroupRepository groups,
    IExpenseRepository expenses,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可・割り勘計算後に支出を保存する。
    /// </summary>
    public async Task<CreateExpenseResponse> HandleAsync(
        Guid groupId,
        CreateExpenseRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var group = await groups.GetByIdAsync(groupId, cancellationToken)
            ?? throw new NotFoundException("グループが見つかりません");

        var memberIds = group.MemberUserIds.ToHashSet();
        var (expense, shareRatios) = ExpenseShareBuilder.Build(
            groupId,
            request.PaidByUserId,
            request.Category,
            request.SplitType,
            request.Amount,
            request.Shares,
            memberIds);

        await expenses.AddAsync(expense, request.Description, request.ExpenseDate, shareRatios, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(expense, request.Description, request.ExpenseDate);
    }

    private static CreateExpenseResponse ToResponse(Expense expense, string description, DateOnly expenseDate) =>
        new(
            expense.Id,
            expense.GroupId,
            expense.Amount,
            expense.PayerId,
            expense.Category,
            description,
            expenseDate,
            expense.SplitType,
            expense.IsSettled,
            expense.Shares.Select(s => new ExpenseShareResponse(s.UserId, s.Amount)).ToList());
}
