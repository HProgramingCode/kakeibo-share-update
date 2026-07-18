using KakeiboShare.Domain.Users;

namespace KakeiboShare.Application.Common.Repositories;

/// <summary>
/// ユーザー集約の永続化。Email は DB 一意制約で保証する。
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// ID でユーザーを取得する。
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// メールアドレス（トリム済み一致）でユーザーを取得する。
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// メールアドレスが既に登録済みか。
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 新規ユーザーを追加する（SaveChanges は呼び出し側）。
    /// </summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// 複数 ID でユーザーを一括取得する。
    /// </summary>
    Task<IReadOnlyDictionary<Guid, User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
