using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Users;

namespace KakeiboShare.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void 生成するとメール_名前_ハッシュを保持する()
    {
        var user = User.Create("a@example.com", "たろう", "hashed-pw");

        Assert.Equal("a@example.com", user.Email);
        Assert.Equal("たろう", user.Name);
        Assert.Equal("hashed-pw", user.PasswordHash);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void メールが空なら例外()
    {
        Assert.Throws<DomainException>(() => User.Create("  ", "たろう", "hashed-pw"));
    }

    [Fact]
    public void 名前が空なら例外()
    {
        Assert.Throws<DomainException>(() => User.Create("a@example.com", " ", "hashed-pw"));
    }

    [Fact]
    public void パスワードハッシュが空なら例外_平文保存の防止()
    {
        Assert.Throws<DomainException>(() => User.Create("a@example.com", "たろう", ""));
    }
}
