using KakeiboShare.Domain.Settlements;
using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KakeiboShare.Infrastructure.Persistence.Configurations.Settlements;

/// <summary>
/// SettlementEntity の EF Core エンティティマッピング設定。
/// グループとのカスケード削除を定義する。
/// </summary>
public sealed class SettlementConfiguration : IEntityTypeConfiguration<SettlementEntity>
{
    public void Configure(EntityTypeBuilder<SettlementEntity> builder)
    {
        builder.ToTable("Settlements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(x => x.SettledAt).IsRequired();

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Settlements)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
