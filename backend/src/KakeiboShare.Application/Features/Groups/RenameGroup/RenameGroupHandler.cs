using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Application.Features.Groups.RenameGroup;

/// <summary>
/// グループ名変更リクエスト。
/// </summary>
public sealed record RenameGroupRequest(string Name);

/// <summary>
/// グループ名変更レスポンス。
/// </summary>
public sealed record RenameGroupResponse(Guid Id, string Name);

/// <summary>
/// グループ名を更新する。
/// </summary>
public sealed class RenameGroupHandler(
    IGroupRepository groups,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可後にグループ名を変更する。
    /// </summary>
    public async Task<RenameGroupResponse> HandleAsync(
        Guid groupId,
        RenameGroupRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var group = await groups.GetByIdAsync(groupId, cancellationToken)
            ?? throw new NotFoundException("グループが見つかりません");

        group.Rename(request.Name);
        await groups.UpdateAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RenameGroupResponse(group.Id, group.Name);
    }
}
