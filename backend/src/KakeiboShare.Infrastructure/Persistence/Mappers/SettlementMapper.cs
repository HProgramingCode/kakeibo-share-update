using KakeiboShare.Domain.Settlements;
using KakeiboShare.Infrastructure.Persistence.Entities;

namespace KakeiboShare.Infrastructure.Persistence.Mappers;

/// <summary>
/// SettlementEntity と Domain の Settlement 集約間の変換を行う。
/// </summary>
internal static class SettlementMapper
{
    public static Settlement ToDomain(SettlementEntity entity, IEnumerable<ExpenseEntity> linkedExpenses) =>
        Settlement.Reconstitute(
            entity.Id,
            entity.GroupId,
            entity.SettledAt,
            entity.Transfers.Select(t => new Transfer(t.FromUserId, t.ToUserId, t.Amount)).ToList(),
            linkedExpenses.Select(e => e.Id).ToList());

    public static SettlementEntity ToEntity(Settlement settlement) => new()
    {
        Id = settlement.Id,
        GroupId = settlement.GroupId,
        Status = settlement.Status,
        SettledAt = settlement.SettledAt,
        Transfers = settlement.Transfers.Select(t => new SettlementTransferEntity
        {
            Id = Guid.NewGuid(),
            SettlementId = settlement.Id,
            FromUserId = t.From,
            ToUserId = t.To,
            Amount = t.Amount,
        }).ToList(),
    };
}
