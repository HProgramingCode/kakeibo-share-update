using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Settlements;

namespace KakeiboShare.Domain.Tests.Settlements;

public class MinTransfersTests
{
    // 送金案が純額を正しく解消しているか（各人: 受取 − 支払 == net）
    private static void AssertBalances(IReadOnlyDictionary<Guid, int> net, IReadOnlyList<Transfer> transfers)
    {
        // 各人の純額 = 受取 − 支払（この関係が成り立てば送金で全員0になる）
        var delta = net.Keys.ToDictionary(id => id, _ => 0);
        foreach (var t in transfers)
        {
            Assert.True(t.Amount > 0, "送金額は正である必要がある");
            delta[t.From] -= t.Amount; // 支払った分
            delta[t.To] += t.Amount;   // 受取った分
        }
        foreach (var id in net.Keys)
            Assert.Equal(net[id], delta[id]);
    }

    [Fact]
    public void 二人なら一本の送金()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var net = new Dictionary<Guid, int> { [a] = -500, [b] = 500 }; // a が b に支払う

        var transfers = SettlementCalculator.MinTransfers(net);

        Assert.Single(transfers);
        Assert.Equal(new Transfer(a, b, 500), transfers[0]);
        AssertBalances(net, transfers);
    }

    [Fact]
    public void 純額0の人は送金に現れない()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();
        var net = new Dictionary<Guid, int> { [a] = -500, [b] = 500, [c] = 0 };

        var transfers = SettlementCalculator.MinTransfers(net);

        Assert.Single(transfers);
        Assert.DoesNotContain(transfers, t => t.From == c || t.To == c);
    }

    [Fact]
    public void 一人が二人に支払う_二本()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();
        var net = new Dictionary<Guid, int> { [a] = 300, [b] = 200, [c] = -500 };

        var transfers = SettlementCalculator.MinTransfers(net);

        Assert.Equal(2, transfers.Count);
        AssertBalances(net, transfers);
    }

    [Fact]
    public void 厳密解はn_1本より少なくまとまる()
    {
        // 4人だが 2組に割れるため 2本（素朴なら3本）
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var c = Guid.NewGuid();
        var d = Guid.NewGuid();
        var net = new Dictionary<Guid, int> { [a] = 500, [b] = -500, [c] = 300, [d] = -300 };

        var transfers = SettlementCalculator.MinTransfers(net);

        Assert.Equal(2, transfers.Count);
        AssertBalances(net, transfers);
    }

    [Fact]
    public void 全員0なら送金なし()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var net = new Dictionary<Guid, int> { [a] = 0, [b] = 0 };

        Assert.Empty(SettlementCalculator.MinTransfers(net));
    }

    [Fact]
    public void 純額の総和が0でないなら例外()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        var net = new Dictionary<Guid, int> { [a] = -500, [b] = 400 };

        Assert.Throws<DomainException>(() => SettlementCalculator.MinTransfers(net));
    }

    [Fact]
    public void 上限超過ではgreedyにフォールバックしても収支は一致する()
    {
        // 12人（>10）→ greedy 経路。厳密最小は問わず、収支一致と本数上限のみ検証。
        var ids = Enumerable.Range(0, 12).Select(_ => Guid.NewGuid()).ToArray();
        var net = new Dictionary<Guid, int>();
        for (int i = 0; i < 6; i++) net[ids[i]] = 100;      // 受取 6人
        for (int i = 6; i < 12; i++) net[ids[i]] = -100;    // 支払 6人

        var transfers = SettlementCalculator.MinTransfers(net);

        AssertBalances(net, transfers);
        Assert.True(transfers.Count <= net.Count - 1, "本数は(人数-1)以下に収まる");
    }
}
