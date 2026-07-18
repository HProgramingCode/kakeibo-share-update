namespace KakeiboShare.Application.Common.Exceptions;

/// <summary>
/// 要求されたリソースが存在しない場合に投げる。Phase 5 で HTTP 404 に変換する。
/// </summary>
public sealed class NotFoundException : Exception
{
    /// <summary>
    /// メッセージ付きで例外を生成する。
    /// </summary>
    public NotFoundException(string message) : base(message) { }
}
