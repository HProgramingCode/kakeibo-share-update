namespace KakeiboShare.Infrastructure.Persistence.Entities;

/// <summary>
/// SettlementTransfers テーブルに対応する永続化モデル。
/// 精算時の送金元・送金先と金額を保持する。
/// </summary>
public sealed class SettlementTransferEntity
{
    public Guid Id { get; set; }
    public Guid SettlementId { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public int Amount { get; set; }

    public SettlementEntity Settlement { get; set; } = null!;
    public UserEntity FromUser { get; set; } = null!;
    public UserEntity ToUser { get; set; } = null!;
}
