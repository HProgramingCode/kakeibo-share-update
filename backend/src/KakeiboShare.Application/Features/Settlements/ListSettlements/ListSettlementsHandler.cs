using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Application.Features.Settlements.CalculateSettlement;

namespace KakeiboShare.Application.Features.Settlements.ListSettlements;

/// <summary>
/// 精算履歴の1件。
/// </summary>
public sealed record ListSettlementsItemResponse(
    Guid SettlementId,
    DateTimeOffset SettledAt,
    IReadOnlyList<TransferResponse> Transfers);

/// <summary>
/// グループの精算履歴を取得する。
/// </summary>
public sealed class ListSettlementsHandler(
    ISettlementRepository settlements,
    IGroupMembershipChecker membershipChecker)
{
    /// <summary>
    /// 認可後に精算履歴を返す。
    /// </summary>
    public async Task<IReadOnlyList<ListSettlementsItemResponse>> HandleAsync(
        Guid groupId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var list = await settlements.ListByGroupAsync(groupId, cancellationToken);
        return list
            .Select(s => new ListSettlementsItemResponse(
                s.Id,
                s.SettledAt,
                s.Transfers.Select(t => new TransferResponse(t.From, t.To, t.Amount)).ToList()))
            .ToList();
    }
}
