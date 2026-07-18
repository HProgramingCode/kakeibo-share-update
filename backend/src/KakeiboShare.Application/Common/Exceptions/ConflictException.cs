namespace KakeiboShare.Application.Common.Exceptions;

/// <summary>
/// リソースの状態が操作と矛盾する場合に投げる。Phase 5 で HTTP 409 に変換する。
/// </summary>
public sealed class ConflictException : Exception
{
    /// <summary>
    /// メッセージ付きで例外を生成する。
    /// </summary>
    public ConflictException(string message) : base(message) { }
}
