using KakeiboShare.Application.Common;
using KakeiboShare.Infrastructure.Persistence;

namespace KakeiboShare.Infrastructure.Repositories;

/// <summary>
/// IUnitOfWork の EF Core 実装。
/// AppDbContext の変更を単一トランザクション境界でコミットする。
/// </summary>
internal sealed class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    /// <summary>保留中の変更をデータベースに反映する。</summary>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        db.SaveChangesAsync(cancellationToken);
}
