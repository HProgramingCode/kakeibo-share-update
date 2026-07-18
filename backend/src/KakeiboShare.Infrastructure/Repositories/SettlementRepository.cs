using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Settlements;
using KakeiboShare.Infrastructure.Persistence;
using KakeiboShare.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace KakeiboShare.Infrastructure.Repositories;

/// <summary>
/// ISettlementRepository の EF Core 実装。
/// 精算集約と送金を SettlementEntity 経由で読み書きする。
/// </summary>
internal sealed class SettlementRepository(AppDbContext db) : ISettlementRepository
{
    /// <summary>ID で精算・送金一覧・対象支出 ID を取得する。</summary>
    public async Task<Settlement?> GetWithTransfersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Settlements.AsNoTracking()
            .Include(x => x.Transfers)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        var expenses = await db.Expenses.AsNoTracking()
            .Where(x => x.SettlementId == id)
            .ToListAsync(cancellationToken);

        return SettlementMapper.ToDomain(entity, expenses);
    }

    /// <summary>グループの精算履歴（新しい順）。</summary>
    public async Task<IReadOnlyList<Settlement>> ListByGroupAsync(Guid groupId,
        CancellationToken cancellationToken = default)
    {
        var entities = await db.Settlements.AsNoTracking()
            .Include(x => x.Transfers)
            .Where(x => x.GroupId == groupId)
            .OrderByDescending(x => x.SettledAt)
            .ToListAsync(cancellationToken);

        if (entities.Count == 0) return [];

        var settlementIds = entities.Select(x => x.Id).ToList();
        var expenses = await db.Expenses.AsNoTracking()
            .Where(x => x.SettlementId != null && settlementIds.Contains(x.SettlementId.Value))
            .ToListAsync(cancellationToken);

        return entities
            .Select(e => SettlementMapper.ToDomain(e, expenses.Where(x => x.SettlementId == e.Id)))
            .ToList();
    }

    /// <summary>精算と送金を保存し、対象支出に SettlementId を付与する。</summary>
    public async Task AddAsync(Settlement settlement, CancellationToken cancellationToken = default)
    {
        await db.Settlements.AddAsync(SettlementMapper.ToEntity(settlement), cancellationToken);

        var expenseIds = settlement.TargetExpenseIds.ToHashSet();
        var expenses = await db.Expenses
            .Where(x => expenseIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        foreach (var expense in expenses)
            expense.SettlementId = settlement.Id;
    }
}
