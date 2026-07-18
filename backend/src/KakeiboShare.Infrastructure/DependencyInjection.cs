using KakeiboShare.Application.Common;
using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Application.Common.Authorization;
using KakeiboShare.Application.Common.Repositories;
using KakeiboShare.Infrastructure.Auth;
using KakeiboShare.Infrastructure.Authorization;
using KakeiboShare.Infrastructure.Persistence;
using KakeiboShare.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KakeiboShare.Infrastructure;

/// <summary>
/// Infrastructure 層の DI 登録。DbContext とリポジトリ実装をスコープ付きで追加する。
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// PostgreSQL（Npgsql）と永続化リポジトリをサービスコレクションに登録する。
    /// </summary>
    /// <param name="services">ASP.NET Core のサービスコレクション。</param>
    /// <param name="configuration">接続文字列 ConnectionStrings:Default を参照する。</param>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<ISettlementRepository, SettlementRepository>();
        services.AddScoped<IGroupMembershipChecker, GroupMembershipChecker>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();

        return services;
    }
}
