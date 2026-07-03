using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Groups;

namespace KakeiboShare.Domain.Tests.Groups;

public class GroupTests
{
    private static readonly Guid Creator = Guid.NewGuid();
    private static readonly Guid NewUser = Guid.NewGuid();

    [Fact]
    public void 生成すると作成者が唯一のメンバーで招待コードを持つ()
    {
        var group = Group.Create("家族の家計", Creator);

        Assert.Equal("家族の家計", group.Name);
        Assert.Single(group.MemberUserIds);
        Assert.Contains(Creator, group.MemberUserIds);
        Assert.False(string.IsNullOrWhiteSpace(group.InviteCode.Value));
    }

    [Fact]
    public void 名前が空なら例外()
    {
        Assert.Throws<DomainException>(() => Group.Create("  ", Creator));
    }

    [Fact]
    public void メンバーを追加できる()
    {
        var group = Group.Create("家族の家計", Creator);

        group.AddMember(NewUser);

        Assert.Equal(2, group.MemberUserIds.Count);
        Assert.Contains(NewUser, group.MemberUserIds);
    }

    [Fact]
    public void 同じユーザーの二重参加は例外()
    {
        var group = Group.Create("家族の家計", Creator);
        group.AddMember(NewUser);

        Assert.Throws<DomainException>(() => group.AddMember(NewUser));
    }

    [Fact]
    public void 招待コードを再発行すると旧コードは変わる()
    {
        var group = Group.Create("家族の家計", Creator);
        var old = group.InviteCode;

        group.RegenerateInvite();

        Assert.NotEqual(old, group.InviteCode);
    }
}
