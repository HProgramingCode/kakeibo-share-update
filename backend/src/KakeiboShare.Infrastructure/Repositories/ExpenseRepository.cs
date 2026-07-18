using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Expenses;
using KakeiboShare.Infrastructure.Persistence;
using KakeiboShare.Infrastructure.Persistence.Entities;
using KakeiboShare.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace KakeiboShare.Infrastructure.Repositories;

/// <summary>
/// IExpenseRepository の EF Core 実装。
/// 支出集約と Shares を ExpenseEntity 経由で読み書きする。
/// </summary>
internal sealed class ExpenseRepository(AppDbContext db) : IExpenseRepository
{
    /// <summary>ID で支出と Shares を取得する。</summary>
    public async Task<Expense?> GetWithSharesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Expenses.AsNoTracking()
            .Include(x => x.Shares)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null ? null : ExpenseMapper.ToDomain(entity);
    }

    /// <summary>ID で支出詳細（Description / ExpenseDate / ShareRatio 含む）を取得する。</summary>
    public async Task<ExpenseDetail?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Expenses.AsNoTracking()
            .Include(x => x.Shares)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        var shareRatios = entity.Shares.ToDictionary(s => s.UserId, s => s.ShareRatio);
        return new ExpenseDetail(
            ExpenseMapper.ToDomain(entity),
            entity.Description,
            entity.ExpenseDate,
            shareRatios);
    }

    /// <summary>
    /// グループの支出一覧。settled: true=精算済 / false=未精算 / null=全件。
    /// </summary>
    public async Task<IReadOnlyList<Expense>> ListByGroupAsync(Guid groupId, bool? settled,
        CancellationToken cancellationToken = default)
    {
        var query = db.Expenses.AsNoTracking()
            .Include(x => x.Shares)
            .Where(x => x.GroupId == groupId);

        query = settled switch
        {
            true => query.Where(x => x.SettlementId != null),
            false => query.Where(x => x.SettlementId == null),
            _ => query,
        };

        var entities = await query.OrderByDescending(x => x.ExpenseDate).ToListAsync(cancellationToken);
        return entities.Select(ExpenseMapper.ToDomain).ToList();
    }

    /// <summary>グループの支出一覧サマリ（API 一覧用）。</summary>
    public async Task<IReadOnlyList<ExpenseSummary>> ListSummariesByGroupAsync(Guid groupId, bool? settled,
        CancellationToken cancellationToken = default)
    {
        var query = db.Expenses.AsNoTracking().Where(x => x.GroupId == groupId);

        query = settled switch
        {
            true => query.Where(x => x.SettlementId != null),
            false => query.Where(x => x.SettlementId == null),
            _ => query,
        };

        var entities = await query.OrderByDescending(x => x.ExpenseDate).ToListAsync(cancellationToken);
        return entities
            .Select(e => new ExpenseSummary(
                e.Id,
                e.Amount,
                e.PaidByUserId,
                e.Category,
                e.ExpenseDate,
                e.SettlementId != null))
            .ToList();
    }

    /// <summary>新規支出と Shares を追加する。shareRatios は Ratio 時のみ値を持つ。</summary>
    public async Task AddAsync(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, CancellationToken cancellationToken = default)
    {
        await db.Expenses.AddAsync(
            ExpenseMapper.ToEntity(expense, description, expenseDate, shareRatios, DateTimeOffset.UtcNow),
            cancellationToken);
    }

    /// <summary>未精算支出を更新する（Shares は全置換）。</summary>
    public async Task UpdateAsync(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, CancellationToken cancellationToken = default)
    {
        var entity = await db.Expenses
            .Include(x => x.Shares)
            .FirstOrDefaultAsync(x => x.Id == expense.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Expense {expense.Id} not found.");

        entity.PaidByUserId = expense.PayerId;
        entity.Amount = expense.Amount;
        entity.Category = expense.Category;
        entity.Description = description;
        entity.ExpenseDate = expenseDate;
        entity.SplitType = expense.SplitType;
        entity.SettlementId = expense.SettlementId;

        db.ExpenseShares.RemoveRange(entity.Shares);
        entity.Shares = expense.Shares.Select(s => new ExpenseShareEntity
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            UserId = s.UserId,
            ShareRatio = shareRatios.TryGetValue(s.UserId, out var ratio) ? ratio : null,
            ShareAmount = s.Amount,
        }).ToList();
    }

    /// <summary>支出を削除する（Cascade で Shares も削除）。</summary>
    public async Task RemoveAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        var entity = await db.Expenses.FirstOrDefaultAsync(x => x.Id == expense.Id, cancellationToken);
        if (entity is not null) db.Expenses.Remove(entity);
    }
}
