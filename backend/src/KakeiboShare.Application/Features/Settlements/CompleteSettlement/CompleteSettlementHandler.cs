using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Settlements;
using KakeiboShare.Application.Features.Settlements.CalculateSettlement;

namespace KakeiboShare.Application.Features.Settlements.CompleteSettlement;

/// <summary>
/// 精算確定レスポンス。
/// </summary>
public sealed record CompleteSettlementResponse(
    Guid SettlementId,
    DateTimeOffset SettledAt,
    IReadOnlyList<TransferResponse> Transfers);

/// <summary>
/// 精算を確定し、対象支出をロックする。
/// </summary>
public sealed class CompleteSettlementHandler(
    IExpenseRepository expenses,
    ISettlementRepository settlements,
    IGroupMembershipChecker membershipChecker,
    IUnitOfWork unitOfWork)
{
    /// <summary>
    /// 認可後に精算を原子的に確定する。
    /// </summary>
    public async Task<CompleteSettlementResponse> HandleAsync(
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
        var targetExpenseIds = unsettled.Select(e => e.Id).ToList();

        var settlement = Settlement.Complete(groupId, transfers, targetExpenseIds);

        foreach (var expense in unsettled)
            expense.AttachToSettlement(settlement.Id);

        await settlements.AddAsync(settlement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CompleteSettlementResponse(
            settlement.Id,
            settlement.SettledAt,
            settlement.Transfers
                .Select(t => new TransferResponse(t.From, t.To, t.Amount))
                .ToList());
    }
}
