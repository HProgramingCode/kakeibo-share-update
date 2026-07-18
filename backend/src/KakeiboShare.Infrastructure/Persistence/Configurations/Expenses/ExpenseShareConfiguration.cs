using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KakeiboShare.Infrastructure.Persistence.Configurations.Expenses;

/// <summary>
/// ExpenseShareEntity の EF Core エンティティマッピング設定。
/// 支出・ユーザーとの関連を定義する。
/// </summary>
public sealed class ExpenseShareConfiguration : IEntityTypeConfiguration<ExpenseShareEntity>
{
    public void Configure(EntityTypeBuilder<ExpenseShareEntity> builder)
    {
        builder.ToTable("ExpenseShares");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ShareRatio).IsRequired(false);
        builder.Property(x => x.ShareAmount).IsRequired();

        builder.HasOne(x => x.Expense)
            .WithMany(x => x.Shares)
            .HasForeignKey(x => x.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
