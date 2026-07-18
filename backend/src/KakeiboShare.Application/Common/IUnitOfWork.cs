namespace KakeiboShare.Application.Common;

/// <summary>
/// 永続化の単一コミット境界。ユースケース末尾で1回 SaveChanges する。
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// 保留中の変更をデータベースに反映する。
    /// </summary>
    /// <returns>影響を受けた行数。</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
