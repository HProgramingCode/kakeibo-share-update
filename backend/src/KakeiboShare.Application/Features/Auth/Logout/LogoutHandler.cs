namespace KakeiboShare.Application.Features.Auth.Logout;

/// <summary>
/// ログアウト（サーバー無状態。クライアントがトークンを破棄する）。
/// </summary>
public sealed class LogoutHandler
{
    /// <summary>
    /// サーバー側の処理は不要。
    /// </summary>
    public Task HandleAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}
