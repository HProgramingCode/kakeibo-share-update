namespace KakeiboShare.Application.Common.Exceptions;

/// <summary>
/// 認証に失敗した場合に投げる。Phase 5 で HTTP 401 に変換する。
/// </summary>
public sealed class UnauthorizedException : Exception
{
    /// <summary>
    /// メッセージ付きで例外を生成する。
    /// </summary>
    public UnauthorizedException(string message) : base(message) { }
}
