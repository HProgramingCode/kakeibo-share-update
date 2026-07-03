using KakeiboShare.Domain.Common;

namespace KakeiboShare.Domain.Groups;

/// <summary>グループ招待コード（値オブジェクト）。無期限だが再発行で置き換わる。</summary>
public readonly record struct InviteCode(string Value)
{
    public static InviteCode Generate() => new(Guid.NewGuid().ToString("N")[..8].ToUpperInvariant());
}
