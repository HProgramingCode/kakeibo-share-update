namespace KakeiboShare.Domain.Expenses;

/// <summary>支出カテゴリ（固定enum・マスタは持たない）。DBには文字列で保存する。</summary>
public enum Category
{
    Food,          // 食費
    DailyGoods,    // 日用品
    Utilities,     // 光熱費
    Transport,     // 交通
    Entertainment, // 娯楽
    Other,         // その他
}
