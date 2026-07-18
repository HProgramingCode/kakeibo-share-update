using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Domain.Users;

namespace KakeiboShare.Application.Features.Auth;

/// <summary>
/// 認証 API 共通のユーザー DTO。
/// </summary>
public sealed record AuthUserResponse(Guid Id, string Name, string Email);

/// <summary>
/// 認証成功時の共通レスポンス。
/// </summary>
public sealed record AuthResponse(string Token, AuthUserResponse User);
