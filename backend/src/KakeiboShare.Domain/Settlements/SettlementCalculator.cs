using KakeiboShare.Domain.Common;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Domain.Settlements;

/// <summary>精算計算（純粋関数・ADR-0001）。純額（総和0）から送金本数最小の送金案を求める。</summary>
public static class SettlementCalculator
{
    /// <summary>未精算支出群から各メンバーの純額（立替 − 負担）を求める。総和は必ず0。</summary>
    public static IReadOnlyDictionary<Guid, int> NetBalances(IEnumerable<Expense> expenses)
    {
        var net = new Dictionary<Guid, int>();
        foreach (var e in expenses)
        {
            net[e.PayerId] = net.GetValueOrDefault(e.PayerId) + e.Amount; // 立替
            foreach (var share in e.Shares)
                net[share.UserId] = net.GetValueOrDefault(share.UserId) - share.Amount; // 負担
        }
        return net;
    }


    /// <summary>厳密な総当たりを行う純額の人数上限。これを超えたら greedy にフォールバック。</summary>
    private const int MaxExact = 10;

    public static IReadOnlyList<Transfer> MinTransfers(IReadOnlyDictionary<Guid, int> net)
    {
        if (net.Values.Sum() != 0)
            throw new DomainException("純額の総和は0である必要があります");

        // 純額0の人は送金に現れないので除外
        var nonzero = net.Where(kv => kv.Value != 0).ToArray();
        if (nonzero.Length == 0) return Array.Empty<Transfer>();

        var ids = nonzero.Select(kv => kv.Key).ToArray();
        var bal = nonzero.Select(kv => kv.Value).ToArray();

        return ids.Length <= MaxExact ? SolveExact(ids, bal, 0)! : SolveGreedy(ids, bal);
    }

    // 総当たり（バックトラック）で送金本数を最小化する。bal は破壊的に使い、呼び出し後に復元する。
    // person `from` の残高を逆符号の誰かへ全額移し、以降は from+1 から再帰する（from は解決済みとみなす）。
    // 相殺で符号が反転し逆符号の相手が尽きた枝は「デッド」として null を返し、上位で捨てる。
    private static List<Transfer>? SolveExact(Guid[] ids, int[] bal, int from)
    {
        while (from < bal.Length && bal[from] == 0) from++;
        if (from == bal.Length) return new List<Transfer>(); // 全員精算済み（コスト0の有効解）

        List<Transfer>? best = null;
        for (var i = from + 1; i < bal.Length; i++)
        {
            if (bal[i] == 0) continue;
            if ((bal[i] > 0) == (bal[from] > 0)) continue; // 逆符号のみ相殺できる

            var amount = Math.Abs(bal[from]);
            var transfer = bal[from] < 0
                ? new Transfer(ids[from], ids[i], amount)  // from が支払う
                : new Transfer(ids[i], ids[from], amount); // from が受取る

            bal[i] += bal[from];
            var sub = SolveExact(ids, bal, from + 1);
            bal[i] -= bal[from];

            if (sub is not null && (best is null || sub.Count + 1 < best.Count))
            {
                sub.Insert(0, transfer);
                best = sub;
            }
        }
        return best; // 有効な枝が無ければ null（デッド）
    }

    // 貪欲法: 最大の支払者→最大の受取者へ順次割当。最悪でも (人数-1) 本に収まる。
    private static List<Transfer> SolveGreedy(Guid[] ids, int[] bal)
    {
        var debtors = new List<(Guid id, int amt)>();   // amt: 支払う額（正）
        var creditors = new List<(Guid id, int amt)>(); // amt: 受取る額（正）
        for (var i = 0; i < ids.Length; i++)
        {
            if (bal[i] < 0) debtors.Add((ids[i], -bal[i]));
            else if (bal[i] > 0) creditors.Add((ids[i], bal[i]));
        }
        debtors.Sort((x, y) => y.amt.CompareTo(x.amt));
        creditors.Sort((x, y) => y.amt.CompareTo(x.amt));

        var transfers = new List<Transfer>();
        int di = 0, ci = 0;
        while (di < debtors.Count && ci < creditors.Count)
        {
            var pay = Math.Min(debtors[di].amt, creditors[ci].amt);
            transfers.Add(new Transfer(debtors[di].id, creditors[ci].id, pay));
            debtors[di] = (debtors[di].id, debtors[di].amt - pay);
            creditors[ci] = (creditors[ci].id, creditors[ci].amt - pay);
            if (debtors[di].amt == 0) di++;
            if (creditors[ci].amt == 0) ci++;
        }
        return transfers;
    }
}
