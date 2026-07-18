using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Users;
using KakeiboShare.Infrastructure.Persistence;
using KakeiboShare.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace KakeiboShare.Infrastructure.Repositories;

/// <summary>
/// IUserRepository の EF Core 実装。
/// ユーザー集約の読み書きを UserEntity 経由で行う。
/// </summary>
internal sealed class UserRepository(AppDbContext db) : IUserRepository
{
    /// <summary>ID でユーザーを取得する。</summary>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null ? null : UserMapper.ToDomain(entity);
    }

    /// <summary>メールアドレス（トリム済み一致）でユーザーを取得する。</summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim();
        var entity = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == normalized, cancellationToken);
        return entity is null ? null : UserMapper.ToDomain(entity);
    }

    /// <summary>メールアドレスが既に登録済みか。</summary>
    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(x => x.Email == email.Trim(), cancellationToken);

    /// <summary>新規ユーザーを追加する（SaveChanges は呼び出し側）。</summary>
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await db.Users.AddAsync(UserMapper.ToEntity(user, DateTimeOffset.UtcNow), cancellationToken);
    }

    /// <summary>複数 ID でユーザーを一括取得する。</summary>
    public async Task<IReadOnlyDictionary<Guid, User>> GetByIdsAsync(IEnumerable<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0) return new Dictionary<Guid, User>();

        var entities = await db.Users.AsNoTracking()
            .Where(x => idList.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return entities.ToDictionary(x => x.Id, UserMapper.ToDomain);
    }
}
