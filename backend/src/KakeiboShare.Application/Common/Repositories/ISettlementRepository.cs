using KakeiboShare.Domain.Settlements;

namespace KakeiboShare.Application.Common.Repositories;

/// <summary>
/// 精算集約の永続化。確定時に対象 Expense の SettlementId を更新してロックする。
/// </summary>
public interface ISettlementRepository
{
    /// <summary>ID で精算・送金一覧・対象支出 ID を取得する。</summary>
    Task<Settlement?> GetWithTransfersAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>グループの精算履歴（新しい順）。</summary>
    Task<IReadOnlyList<Settlement>> ListByGroupAsync(Guid groupId, CancellationToken cancellationToken = default);

    /// <summary>精算と送金を保存し、対象支出に SettlementId を付与する。</summary>
    Task AddAsync(Settlement settlement, CancellationToken cancellationToken = default);
}
