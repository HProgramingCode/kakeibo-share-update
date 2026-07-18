using KakeiboShare.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace KakeiboShare.Infrastructure.Persistence;

/// <summary>
/// アプリ全体の EF Core DbContext。テーブル定義は Configurations/ と database-design.md に準拠。
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<GroupEntity> Groups => Set<GroupEntity>();
    public DbSet<GroupMemberEntity> GroupMembers => Set<GroupMemberEntity>();
    public DbSet<ExpenseEntity> Expenses => Set<ExpenseEntity>();
    public DbSet<ExpenseShareEntity> ExpenseShares => Set<ExpenseShareEntity>();
    public DbSet<SettlementEntity> Settlements => Set<SettlementEntity>();
    public DbSet<SettlementTransferEntity> SettlementTransfers => Set<SettlementTransferEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
