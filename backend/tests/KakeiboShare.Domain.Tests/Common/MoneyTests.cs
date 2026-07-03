using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Tests.Common;

public class MoneyTests
{
    [Fact]
    public void 円の値を保持する()
    {
        var money = new Money(1000);
        Assert.Equal(1000, money.Yen);
    }

    [Fact]
    public void 加算できる()
    {
        Assert.Equal(new Money(500), new Money(300) + new Money(200));
    }

    [Fact]
    public void 減算は負数も許容する_純額で使うため()
    {
        Assert.Equal(new Money(-200), new Money(300) - new Money(500));
    }
}
