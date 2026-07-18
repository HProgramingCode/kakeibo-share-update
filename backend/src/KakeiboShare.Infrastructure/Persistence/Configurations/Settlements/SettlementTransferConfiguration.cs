using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KakeiboShare.Infrastructure.Persistence.Configurations.Settlements;

/// <summary>
/// SettlementTransferEntity の EF Core エンティティマッピング設定。
/// 精算・送金元・送金先との関連を定義する。
/// </summary>
public sealed class SettlementTransferConfiguration : IEntityTypeConfiguration<SettlementTransferEntity>
{
    public void Configure(EntityTypeBuilder<SettlementTransferEntity> builder)
    {
        builder.ToTable("SettlementTransfers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired();

        builder.HasOne(x => x.Settlement)
            .WithMany(x => x.Transfers)
            .HasForeignKey(x => x.SettlementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.FromUser)
            .WithMany()
            .HasForeignKey(x => x.FromUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ToUser)
            .WithMany()
            .HasForeignKey(x => x.ToUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
