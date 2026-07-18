namespace KakeiboShare.Application.Common.Exceptions;

/// <summary>
/// 操作ユーザーが対象リソースへのアクセス権を持たない場合に投げる。Phase 5 で HTTP 403 に変換する。
/// </summary>
public sealed class ForbiddenException : Exception
{
    /// <summary>
    /// メッセージ付きで例外を生成する。
    /// </summary>
    public ForbiddenException(string message) : base(message) { }
}
