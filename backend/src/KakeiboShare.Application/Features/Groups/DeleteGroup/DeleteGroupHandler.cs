using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Application.Features.Groups.DeleteGroup;

/// <summary>
/// グループを削除する。
/// </summary>
public sealed class DeleteGroupHandler(
    IGroupRepository groups,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可後にグループを削除する。
    /// </summary>
    public async Task HandleAsync(
        Guid groupId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);
        await groups.RemoveAsync(groupId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
