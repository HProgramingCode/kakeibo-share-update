using KakeiboShare.Domain.Expenses;
using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KakeiboShare.Infrastructure.Persistence.Configurations.Expenses;

/// <summary>
/// ExpenseEntity の EF Core エンティティマッピング設定。
/// グループ・支払者・精算との関連を定義する。
/// </summary>
public sealed class ExpenseConfiguration : IEntityTypeConfiguration<ExpenseEntity>
{
    public void Configure(EntityTypeBuilder<ExpenseEntity> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500).IsRequired();
        builder.Property(x => x.ExpenseDate).IsRequired();
        builder.Property(x => x.SplitType).HasConversion<string>().HasMaxLength(16).IsRequired();
        builder.Property(x => x.SettlementId).IsRequired(false);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.PaidByUser)
            .WithMany()
            .HasForeignKey(x => x.PaidByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Settlement)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.SettlementId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
