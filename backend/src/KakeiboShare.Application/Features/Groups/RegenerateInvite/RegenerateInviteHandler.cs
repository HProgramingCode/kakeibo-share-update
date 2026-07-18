using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Application.Features.Groups.RegenerateInvite;

/// <summary>
/// 招待コード再発行レスポンス。
/// </summary>
public sealed record RegenerateInviteResponse(string InviteCode);

/// <summary>
/// グループの招待コードを再発行する。
/// </summary>
public sealed class RegenerateInviteHandler(
    IGroupRepository groups,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可後に新しい招待コードを発行する。
    /// </summary>
    public async Task<RegenerateInviteResponse> HandleAsync(
        Guid groupId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var group = await groups.GetByIdAsync(groupId, cancellationToken)
            ?? throw new NotFoundException("グループが見つかりません");

        group.RegenerateInvite();
        await groups.UpdateAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegenerateInviteResponse(group.InviteCode.Value);
    }
}
