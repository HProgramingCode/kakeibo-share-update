using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Application.Features.Groups.GetGroup;

/// <summary>
/// グループ詳細のメンバー1件。
/// </summary>
public sealed record GroupMemberResponse(Guid UserId, string Name);

/// <summary>
/// グループ詳細レスポンス。
/// </summary>
public sealed record GetGroupResponse(
    Guid Id,
    string Name,
    string InviteCode,
    IReadOnlyList<GroupMemberResponse> Members);

/// <summary>
/// グループ詳細とメンバー一覧を取得する。
/// </summary>
public sealed class GetGroupHandler(
    IGroupRepository groups,
    IGroupMembershipChecker membershipChecker)
{
    /// <summary>
    /// 認可後にグループ詳細を返す。
    /// </summary>
    public async Task<GetGroupResponse> HandleAsync(
        Guid groupId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var group = await groups.GetByIdAsync(groupId, cancellationToken)
            ?? throw new NotFoundException("グループが見つかりません");

        var members = await groups.ListMemberDetailsAsync(groupId, cancellationToken);
        var memberResponses = members
            .Select(m => new GroupMemberResponse(m.UserId, m.Name))
            .ToList();

        return new GetGroupResponse(group.Id, group.Name, group.InviteCode.Value, memberResponses);
    }
}
