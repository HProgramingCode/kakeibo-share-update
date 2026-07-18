using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Application.Features.Groups.ListMembers;

/// <summary>
/// メンバー一覧の1件。
/// </summary>
public sealed record ListMembersItemResponse(Guid UserId, string Name, DateTimeOffset JoinedAt);

/// <summary>
/// グループのメンバー一覧を取得する。
/// </summary>
public sealed class ListMembersHandler(
    IGroupRepository groups,
    IGroupMembershipChecker membershipChecker)
{
    /// <summary>
    /// 認可後にメンバー一覧を返す。
    /// </summary>
    public async Task<IReadOnlyList<ListMembersItemResponse>> HandleAsync(
        Guid groupId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var members = await groups.ListMemberDetailsAsync(groupId, cancellationToken);
        return members
            .Select(m => new ListMembersItemResponse(m.UserId, m.Name, m.JoinedAt))
            .ToList();
    }
}
