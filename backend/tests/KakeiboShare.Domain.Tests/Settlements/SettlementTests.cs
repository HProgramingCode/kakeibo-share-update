using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Settlements;

namespace KakeiboShare.Domain.Tests.Settlements;

public class SettlementTests
{
    private static readonly Guid GroupId = Guid.NewGuid();
    private static readonly Guid A = Guid.NewGuid();
    private static readonly Guid B = Guid.NewGuid();

    private static readonly Transfer[] Transfers = { new(A, B, 500) };
    private static readonly Guid[] Targets = { Guid.NewGuid(), Guid.NewGuid() };

    [Fact]
    public void 確定するとCompletedで送金と対象を保持する()
    {
        var settlement = Settlement.Complete(GroupId, Transfers, Targets);

        Assert.Equal(SettlementStatus.Completed, settlement.Status);
        Assert.Equal(GroupId, settlement.GroupId);
        Assert.Single(settlement.Transfers);
        Assert.Equal(2, settlement.TargetExpenseIds.Count);
        Assert.NotEqual(Guid.Empty, settlement.Id);
    }

    [Fact]
    public void 対象支出が無いなら例外()
    {
        Assert.Throws<DomainException>(() => Settlement.Complete(GroupId, Transfers, Array.Empty<Guid>()));
    }

    [Fact]
    public void 送金額が0以下なら例外()
    {
        var bad = new Transfer[] { new(A, B, 0) };
        Assert.Throws<DomainException>(() => Settlement.Complete(GroupId, bad, Targets));
    }
}
