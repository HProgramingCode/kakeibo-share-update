using KakeiboShare.Application.Common.Repositories;

namespace KakeiboShare.Application.Features.Groups.ListMyGroups;

/// <summary>
/// 自分のグループ一覧の1件。
/// </summary>
public sealed record GroupListItemResponse(Guid Id, string Name, int MemberCount);

/// <summary>
/// ログインユーザーが所属するグループ一覧を返す。
/// </summary>
public sealed class ListMyGroupsHandler(IGroupRepository groups)
{
    /// <summary>
    /// 所属グループ一覧を取得する。
    /// </summary>
    public async Task<IReadOnlyList<GroupListItemResponse>> HandleAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var list = await groups.ListByMemberAsync(currentUserId, cancellationToken);
        return list
            .Select(g => new GroupListItemResponse(g.Id, g.Name, g.MemberUserIds.Count))
            .ToList();
    }
}
