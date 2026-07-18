using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Settlements;

/// <summary>
/// 精算（集約ルート）。確定すると対象支出をロックする。MVPは確定時にのみ作成する。
/// </summary>
public sealed class Settlement
{
    public Guid Id { get; }
    public Guid GroupId { get; }
    public SettlementStatus Status { get; }
    public DateTimeOffset SettledAt { get; }
    public IReadOnlyList<Transfer> Transfers { get; }
    public IReadOnlyList<Guid> TargetExpenseIds { get; }

    private Settlement(Guid id, Guid groupId, DateTimeOffset settledAt,
        IReadOnlyList<Transfer> transfers, IReadOnlyList<Guid> targetExpenseIds)
    {
        Id = id;
        GroupId = groupId;
        Status = SettlementStatus.Completed;
        SettledAt = settledAt;
        Transfers = transfers;
        TargetExpenseIds = targetExpenseIds;
    }

    /// <summary>
    /// 対象支出と送金案から精算を確定する。MVPではこのタイミングでのみ Settlement を作成する。
    /// </summary>
    public static Settlement Complete(Guid groupId, IReadOnlyList<Transfer> transfers, IReadOnlyList<Guid> targetExpenseIds)
    {
        if (targetExpenseIds.Count == 0) throw new DomainException("精算対象の支出がありません");
        if (transfers.Any(t => t.Amount <= 0)) throw new DomainException("送金額は正である必要があります");

        return new Settlement(Guid.NewGuid(), groupId, DateTimeOffset.UtcNow, transfers, targetExpenseIds);
    }

    /// <summary>
    /// 永続化層からの再構成用（Phase 3）。
    /// </summary>
    internal static Settlement Reconstitute(Guid id, Guid groupId, DateTimeOffset settledAt,
        IReadOnlyList<Transfer> transfers, IReadOnlyList<Guid> targetExpenseIds) =>
        new(id, groupId, settledAt, transfers, targetExpenseIds);
}
