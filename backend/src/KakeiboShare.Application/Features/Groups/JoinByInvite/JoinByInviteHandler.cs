using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Common;

namespace KakeiboShare.Application.Features.Groups.JoinByInvite;

/// <summary>
/// 招待コード参加リクエスト。
/// </summary>
public sealed record JoinByInviteRequest(string InviteCode);

/// <summary>
/// 招待コード参加レスポンス。
/// </summary>
public sealed record JoinByInviteResponse(Guid GroupId, Guid UserId);

/// <summary>
/// 招待コードでグループに参加する。
/// </summary>
public sealed class JoinByInviteHandler(IGroupRepository groups, IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 招待コードを検証しメンバーを追加する。
    /// </summary>
    public async Task<JoinByInviteResponse> HandleAsync(
        JoinByInviteRequest request,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        var group = await groups.GetByInviteCodeAsync(request.InviteCode, cancellationToken)
            ?? throw new NotFoundException("招待コードが見つかりません");

        if (group.MemberUserIds.Contains(currentUserId))
            throw new ConflictException("既に参加済みです");

        try
        {
            group.AddMember(currentUserId);
        }
        catch (DomainException ex)
        {
            throw new ConflictException(ex.Message);
        }

        await groups.UpdateAsync(group, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new JoinByInviteResponse(group.Id, currentUserId);
    }
}
