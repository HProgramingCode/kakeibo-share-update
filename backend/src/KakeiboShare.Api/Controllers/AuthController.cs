using KakeiboShare.Application.Features.Auth;
using KakeiboShare.Application.Features.Auth.Login;
using KakeiboShare.Application.Features.Auth.Logout;
using KakeiboShare.Application.Features.Auth.SignUp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KakeiboShare.Api.Controllers;

/// <summary>
/// 認証 API（signup / login / logout）。
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    SignUpHandler signUp,
    LoginHandler login,
    LogoutHandler logout) : ControllerBase
{
    /// <summary>
    /// 新規ユーザーを登録する。
    /// </summary>
    [AllowAnonymous]
    [HttpPost("signup")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<AuthResponse>> SignUp(
        [FromBody] SignUpRequest request,
        CancellationToken cancellationToken)
    {
        var result = await signUp.HandleAsync(request, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>
    /// ログインして JWT を取得する。
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await login.HandleAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// ログアウト（サーバー無状態。クライアントがトークンを破棄する）。
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await logout.HandleAsync(cancellationToken);
        return NoContent();
    }
}
