using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Groups;

namespace KakeiboShare.Application.Features.Groups.CreateGroup;

/// <summary>
/// グループ作成リクエスト。
/// </summary>
public sealed record CreateGroupRequest(string Name);

/// <summary>
/// グループ作成レスポンス。
/// </summary>
public sealed record CreateGroupResponse(Guid Id, string Name, string InviteCode);

/// <summary>
/// グループを新規作成し、作成者を最初のメンバーとして追加する。
/// </summary>
public sealed class CreateGroupHandler(IGroupRepository groups, IUnitOfWork unitOfWork)
{
    /// <summary>
    /// グループを作成して永続化する。
    /// </summary>
    public async Task<CreateGroupResponse> HandleAsync(
        CreateGroupRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var group = Group.Create(request.Name, currentUserId);
        await groups.AddAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateGroupResponse(group.Id, group.Name, group.InviteCode.Value);
    }
}
