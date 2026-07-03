namespace KakeiboShare.Domain.Settlements;

/// <summary>
/// 精算の状態。MVPでは確定時に作成するため Completed のみ。DBには文字列で保存する。
/// </summary>
public enum SettlementStatus
{
    Completed, // 確定済み
}
