namespace KakeiboShare.Application.Common.Authorization;

/// <summary>
/// 操作ユーザーが対象グループのメンバーか検証する。非メンバーは例外。
/// </summary>
public interface IGroupMembershipChecker
{
    /// <summary>
    /// 指定ユーザーがグループメンバーであることを検証する。
    /// </summary>
    Task EnsureMemberAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 支出 ID から所属グループを特定し、メンバーであることを検証する。
    /// </summary>
    /// <returns>支出が属するグループ ID。</returns>
    Task<Guid> EnsureMemberOfExpenseAsync(Guid expenseId, Guid userId, CancellationToken cancellationToken = default);
}
