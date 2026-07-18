using KakeiboShare.Application.Common.Models;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Groups;
using KakeiboShare.Infrastructure.Persistence;
using KakeiboShare.Infrastructure.Persistence.Entities;
using KakeiboShare.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;

namespace KakeiboShare.Infrastructure.Repositories;

/// <summary>
/// IGroupRepository の EF Core 実装。
/// グループ集約とメンバーシップを GroupEntity / GroupMemberEntity 経由で行う。
/// </summary>
internal sealed class GroupRepository(AppDbContext db) : IGroupRepository
{
    /// <summary>ID でグループとメンバー一覧を取得する。</summary>
    public async Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await db.Groups.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        var members = await db.GroupMembers.AsNoTracking()
            .Where(x => x.GroupId == id)
            .ToListAsync(cancellationToken);
        return GroupMapper.ToDomain(entity, members);
    }

    /// <summary>招待コードでグループを取得する（大文字正規化）。</summary>
    public async Task<Group?> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
    {
        var normalized = inviteCode.Trim().ToUpperInvariant();
        var entity = await db.Groups.AsNoTracking()
            .FirstOrDefaultAsync(x => x.InviteCode == normalized, cancellationToken);
        if (entity is null) return null;

        var members = await db.GroupMembers.AsNoTracking()
            .Where(x => x.GroupId == entity.Id)
            .ToListAsync(cancellationToken);
        return GroupMapper.ToDomain(entity, members);
    }

    /// <summary>指定ユーザーが所属するグループ一覧を取得する。</summary>
    public async Task<IReadOnlyList<Group>> ListByMemberAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var groupIds = await db.GroupMembers.AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.GroupId)
            .ToListAsync(cancellationToken);

        if (groupIds.Count == 0) return [];

        var groups = await db.Groups.AsNoTracking()
            .Where(x => groupIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var members = await db.GroupMembers.AsNoTracking()
            .Where(x => groupIds.Contains(x.GroupId))
            .ToListAsync(cancellationToken);

        return groups
            .Select(g => GroupMapper.ToDomain(g, members.Where(m => m.GroupId == g.Id)))
            .ToList();
    }

    /// <summary>新規グループと初期メンバーを追加する。</summary>
    public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        await db.Groups.AddAsync(GroupMapper.ToEntity(group, now), cancellationToken);
        await db.GroupMembers.AddRangeAsync(GroupMapper.ToMemberEntities(group, now), cancellationToken);
    }

    /// <summary>名前・招待コード・メンバー差分を更新する。</summary>
    public async Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
    {
        var entity = await db.Groups.FirstOrDefaultAsync(x => x.Id == group.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Group {group.Id} not found.");

        entity.Name = group.Name;
        entity.InviteCode = group.InviteCode.Value;

        var existingMembers = await db.GroupMembers.Where(x => x.GroupId == group.Id).ToListAsync(cancellationToken);
        var desired = group.MemberUserIds.ToHashSet();
        var existingIds = existingMembers.Select(x => x.UserId).ToHashSet();

        foreach (var member in existingMembers.Where(m => !desired.Contains(m.UserId)))
            db.GroupMembers.Remove(member);

        var now = DateTimeOffset.UtcNow;
        foreach (var userId in desired.Where(id => !existingIds.Contains(id)))
        {
            await db.GroupMembers.AddAsync(new GroupMemberEntity
            {
                Id = Guid.NewGuid(),
                GroupId = group.Id,
                UserId = userId,
                JoinedAt = now,
            }, cancellationToken);
        }
    }

    /// <summary>グループのメンバー詳細（JoinedAt 含む）を取得する。</summary>
    public async Task<IReadOnlyList<GroupMemberDetail>> ListMemberDetailsAsync(Guid groupId,
        CancellationToken cancellationToken = default)
    {
        var members = await db.GroupMembers.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.GroupId == groupId)
            .OrderBy(x => x.JoinedAt)
            .ToListAsync(cancellationToken);

        return members
            .Select(m => new GroupMemberDetail(m.UserId, m.User.Name, m.JoinedAt))
            .ToList();
    }

    /// <summary>グループを削除する（配下は DB カスケード）。</summary>
    public async Task RemoveAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        var entity = await db.Groups.FirstOrDefaultAsync(x => x.Id == groupId, cancellationToken);
        if (entity is not null) db.Groups.Remove(entity);
    }
}
