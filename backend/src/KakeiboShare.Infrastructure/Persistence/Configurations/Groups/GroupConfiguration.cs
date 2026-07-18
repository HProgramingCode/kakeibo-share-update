using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KakeiboShare.Infrastructure.Persistence.Configurations.Groups;

/// <summary>
/// GroupEntity の EF Core エンティティマッピング設定。
/// 招待コードの一意制約を含む。
/// </summary>
public sealed class GroupConfiguration : IEntityTypeConfiguration<GroupEntity>
{
    public void Configure(EntityTypeBuilder<GroupEntity> builder)
    {
        builder.ToTable("Groups");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.InviteCode).HasMaxLength(8).IsRequired();
        builder.HasIndex(x => x.InviteCode).IsUnique();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
