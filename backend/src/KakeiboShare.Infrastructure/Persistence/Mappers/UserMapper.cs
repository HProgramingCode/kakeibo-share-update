using KakeiboShare.Domain.Expenses;
using KakeiboShare.Domain.Groups;
using KakeiboShare.Domain.Settlements;
using KakeiboShare.Domain.Users;
using KakeiboShare.Infrastructure.Persistence.Entities;

namespace KakeiboShare.Infrastructure.Persistence.Mappers;

/// <summary>
/// UserEntity と Domain の User 集約間の変換を行う。
/// </summary>
internal static class UserMapper
{
    public static User ToDomain(UserEntity entity) =>
        User.Reconstitute(entity.Id, entity.Email, entity.Name, entity.PasswordHash);

    public static UserEntity ToEntity(User user, DateTimeOffset createdAt) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        PasswordHash = user.PasswordHash,
        CreatedAt = createdAt,
    };
}
