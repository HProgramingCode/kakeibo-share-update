namespace KakeiboShare.Domain.Common;

/// <summary>ドメインの不変条件・ビジネスルール違反。上位層で適切なHTTPへ変換する。</summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
