using KakeiboShare.Application.Common.Models;
using KakeiboShare.Domain.Groups;

namespace KakeiboShare.Application.Common.Repositories;

/// <summary>
/// グループ集約とメンバーシップの永続化。招待コードは Groups テーブルで一意。
/// </summary>
public interface IGroupRepository
{
    /// <summary>ID でグループとメンバー一覧を取得する。</summary>
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>招待コードでグループを取得する（大文字正規化）。</summary>
    Task<Group?> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default);

    /// <summary>指定ユーザーが所属するグループ一覧を取得する。</summary>
    Task<IReadOnlyList<Group>> ListByMemberAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>グループのメンバー詳細（JoinedAt 含む）を取得する。</summary>
    Task<IReadOnlyList<GroupMemberDetail>> ListMemberDetailsAsync(Guid groupId, CancellationToken cancellationToken = default);

    /// <summary>新規グループと初期メンバーを追加する。</summary>
    Task AddAsync(Group group, CancellationToken cancellationToken = default);

    /// <summary>名前・招待コード・メンバー差分を更新する。</summary>
    Task UpdateAsync(Group group, CancellationToken cancellationToken = default);

    /// <summary>グループを削除する（配下は DB カスケード）。</summary>
    Task RemoveAsync(Guid groupId, CancellationToken cancellationToken = default);
}
