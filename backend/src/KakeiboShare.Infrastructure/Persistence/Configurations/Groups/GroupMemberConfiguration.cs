using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KakeiboShare.Infrastructure.Persistence.Configurations.Groups;

/// <summary>
/// GroupMemberEntity の EF Core エンティティマッピング設定。
/// グループ・ユーザーとの外部キーと複合一意制約を定義する。
/// </summary>
public sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMemberEntity>
{
    public void Configure(EntityTypeBuilder<GroupMemberEntity> builder)
    {
        builder.ToTable("GroupMembers");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.GroupId, x.UserId }).IsUnique();
        builder.Property(x => x.JoinedAt).IsRequired();

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.GroupMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
