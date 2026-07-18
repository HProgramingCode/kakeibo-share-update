using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Infrastructure.Authorization;

/// <summary>
/// グループメンバーシップをリポジトリ経由で検証する。
/// </summary>
internal sealed class GroupMembershipChecker(
    IGroupRepository groups,
    IExpenseRepository expenses) : IGroupMembershipChecker
{
    /// <inheritdoc />
    public async Task EnsureMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        var group = await groups.GetByIdAsync(groupId, cancellationToken);
        if (group is null) throw new NotFoundException("グループが見つかりません");
        if (!group.MemberUserIds.Contains(userId))
            throw new ForbiddenException("グループのメンバーではありません");
    }

    /// <inheritdoc />
    public async Task<Guid> EnsureMemberOfExpenseAsync(Guid expenseId, Guid userId, CancellationToken cancellationToken = default)
    {
        var expense = await expenses.GetWithSharesAsync(expenseId, cancellationToken);
        if (expense is null) throw new NotFoundException("支出が見つかりません");

        await EnsureMemberAsync(expense.GroupId, userId, cancellationToken);
        return expense.GroupId;
    }
}
