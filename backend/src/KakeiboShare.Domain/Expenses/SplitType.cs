namespace KakeiboShare.Domain.Expenses;

/// <summary>割り勘方式。DBには文字列で保存する。</summary>
public enum SplitType
{
    Equal, // 均等割り
    Ratio, // 任意割合
}
