using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Common;

namespace KakeiboShare.Application.Features.Expenses.DeleteExpense;

/// <summary>
/// 未精算支出を削除する。
/// </summary>
public sealed class DeleteExpenseHandler(
    IExpenseRepository expenses,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可後に支出を削除する。
    /// </summary>
    public async Task HandleAsync(
        Guid expenseId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberOfExpenseAsync(expenseId, currentUserId, cancellationToken);

        var expense = await expenses.GetWithSharesAsync(expenseId, cancellationToken)
            ?? throw new NotFoundException("支出が見つかりません");

        try
        {
            expense.EnsureEditable();
        }
        catch (DomainException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await expenses.RemoveAsync(expense, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
