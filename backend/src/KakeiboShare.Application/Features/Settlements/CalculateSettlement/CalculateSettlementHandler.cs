using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Settlements;

namespace KakeiboShare.Application.Features.Settlements.CalculateSettlement;

/// <summary>
/// 純額1件。
/// </summary>
public sealed record BalanceResponse(Guid UserId, int Net);

/// <summary>
/// 送金1件。
/// </summary>
public sealed record TransferResponse(Guid FromUserId, Guid ToUserId, int Amount);

/// <summary>
/// 精算プレビューレスポンス。
/// </summary>
public sealed record CalculateSettlementResponse(
    IReadOnlyList<BalanceResponse> Balances,
    IReadOnlyList<TransferResponse> Transfers,
    IReadOnlyList<Guid> TargetExpenseIds);

/// <summary>
/// 未精算支出から精算プレビューを計算する（DB 保存しない）。
/// </summary>
public sealed class CalculateSettlementHandler(
    IExpenseRepository expenses,
    IGroupMembershipChecker membershipChecker)
{
    /// <summary>
    /// 認可後に純額・最小送金・対象支出 ID を計算する。
    /// </summary>
    public async Task<CalculateSettlementResponse> HandleAsync(
        Guid groupId,
        Guid currentUserId,
        CancellationToken cancellationToken = default)
    {
        await membershipChecker.EnsureMemberAsync(groupId, currentUserId, cancellationToken);

        var unsettled = await expenses.ListByGroupAsync(groupId, settled: false, cancellationToken);
        if (unsettled.Count == 0)
            throw new DomainException("精算対象の未精算支出がありません");

        var balances = SettlementCalculator.NetBalances(unsettled);
        var transfers = SettlementCalculator.MinTransfers(balances);

        return new CalculateSettlementResponse(
            balances.Select(kv => new BalanceResponse(kv.Key, kv.Value)).ToList(),
            transfers.Select(t => new TransferResponse(t.From, t.To, t.Amount)).ToList(),
            unsettled.Select(e => e.Id).ToList());
    }
}
