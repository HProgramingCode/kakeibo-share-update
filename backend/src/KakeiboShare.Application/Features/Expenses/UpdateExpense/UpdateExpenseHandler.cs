using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Expenses;
using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;
using KakeiboShare.Application.Features.Expenses.CreateExpense;

namespace KakeiboShare.Application.Features.Expenses.UpdateExpense;

/// <summary>
/// 支出更新リクエスト（登録と同形）。
/// </summary>
public sealed record UpdateExpenseRequest(
    int Amount,
    Guid PaidByUserId,
    Category Category,
    string Description,
    DateOnly ExpenseDate,
    SplitType SplitType,
    IReadOnlyList<ShareInput> Shares);

/// <summary>
/// 未精算支出を更新する。
/// </summary>
public sealed class UpdateExpenseHandler(
    IGroupRepository groups,
    IExpenseRepository expenses,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可・再計算後に支出を更新する。
    /// </summary>
    public async Task<CreateExpenseResponse> HandleAsync(
        Guid expenseId,
        UpdateExpenseRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberOfExpenseAsync(expenseId, currentUserId, cancellationToken);

        var detail = await expenses.GetDetailAsync(expenseId, cancellationToken)
            ?? throw new NotFoundException("支出が見つかりません");

        var group = await groups.GetByIdAsync(detail.Expense.GroupId, cancellationToken)
            ?? throw new NotFoundException("グループが見つかりません");

        try
        {
            var (expense, shareRatios) = ExpenseShareBuilder.Rebuild(
                detail.Expense,
                request.PaidByUserId,
                request.Category,
                request.SplitType,
                request.Amount,
                request.Shares,
                group.MemberUserIds.ToHashSet());

            await expenses.UpdateAsync(expense, request.Description, request.ExpenseDate, shareRatios, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateExpenseResponse(
                expense.Id,
                expense.GroupId,
                expense.Amount,
                expense.PayerId,
                expense.Category,
                request.Description,
                request.ExpenseDate,
                expense.SplitType,
                expense.IsSettled,
                expense.Shares.Select(s => new ExpenseShareResponse(s.UserId, s.Amount)).ToList());
        }
        catch (DomainException ex) when (detail.Expense.IsSettled)
        {
            throw new ConflictException(ex.Message);
        }
    }
}
