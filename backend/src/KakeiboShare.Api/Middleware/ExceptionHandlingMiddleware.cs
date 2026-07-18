using System.Net;
using System.Text.Json;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Domain.Common;

namespace KakeiboShare.Api.Middleware;

/// <summary>
/// ドメイン/Application 例外を provider 準拠の JSON エラー応答に変換する。
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// リクエストを処理し、未処理例外を HTTP ステータスに変換する。
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        var (statusCode, code) = exception switch
        {
            DomainException => (HttpStatusCode.BadRequest, "VALIDATION_ERROR"),
            NotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
            ForbiddenException => (HttpStatusCode.Forbidden, "FORBIDDEN"),
            ConflictException => (HttpStatusCode.Conflict, "CONFLICT"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR"),
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Unhandled exception");

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var body = new
        {
            error = new
            {
                code,
                message = exception.Message,
            },
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
