using KakeiboShare.Domain.Expenses;
using KakeiboShare.Infrastructure.Persistence.Entities;

namespace KakeiboShare.Infrastructure.Persistence.Mappers;

/// <summary>
/// ExpenseEntity と Domain の Expense 集約間の変換を行う。
/// </summary>
internal static class ExpenseMapper
{
    public static Expense ToDomain(ExpenseEntity entity) =>
        Expense.Reconstitute(
            entity.Id,
            entity.GroupId,
            entity.PaidByUserId,
            entity.Category,
            entity.SplitType,
            entity.Amount,
            entity.Shares.Select(s => new ExpenseShare(s.UserId, s.ShareAmount)).ToList(),
            entity.SettlementId);

    public static ExpenseEntity ToEntity(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, DateTimeOffset createdAt) => new()
    {
        Id = expense.Id,
        GroupId = expense.GroupId,
        PaidByUserId = expense.PayerId,
        Amount = expense.Amount,
        Category = expense.Category,
        Description = description,
        ExpenseDate = expenseDate,
        SplitType = expense.SplitType,
        SettlementId = expense.SettlementId,
        CreatedAt = createdAt,
        Shares = expense.Shares.Select(s => new ExpenseShareEntity
        {
            Id = Guid.NewGuid(),
            ExpenseId = expense.Id,
            UserId = s.UserId,
            ShareRatio = shareRatios.TryGetValue(s.UserId, out var ratio) ? ratio : null,
            ShareAmount = s.Amount,
        }).ToList(),
    };
}
