namespace KakeiboShare.Application.Common.Auth;

/// <summary>
/// JWT 署名・発行者・有効期限の設定。appsettings の Jwt セクションにバインドする。
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// 設定セクション名。
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// 署名鍵（32 文字以上推奨）。
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 発行者。
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// 対象者。
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// 有効期限（日）。
    /// </summary>
    public int ExpiresDays { get; set; } = 7;
}
