using KakeiboShare.Domain.Groups;
using KakeiboShare.Infrastructure.Persistence.Entities;

namespace KakeiboShare.Infrastructure.Persistence.Mappers;

/// <summary>
/// GroupEntity / GroupMemberEntity と Domain の Group 集約間の変換を行う。
/// </summary>
internal static class GroupMapper
{
    public static Group ToDomain(GroupEntity entity, IEnumerable<GroupMemberEntity> members) =>
        Group.Reconstitute(
            entity.Id,
            entity.Name,
            new InviteCode(entity.InviteCode),
            members.Select(m => m.UserId));

    public static GroupEntity ToEntity(Group group, DateTimeOffset createdAt) => new()
    {
        Id = group.Id,
        Name = group.Name,
        InviteCode = group.InviteCode.Value,
        CreatedAt = createdAt,
    };

    public static IEnumerable<GroupMemberEntity> ToMemberEntities(Group group, DateTimeOffset joinedAt) =>
        group.MemberUserIds.Select(userId => new GroupMemberEntity
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            UserId = userId,
            JoinedAt = joinedAt,
        });
}
