using KakeiboShare.Application.Features.Auth.Login;
using KakeiboShare.Application.Features.Auth.Logout;
using KakeiboShare.Application.Features.Auth.SignUp;
using KakeiboShare.Application.Features.Expenses.CreateExpense;
using KakeiboShare.Application.Features.Expenses.DeleteExpense;
using KakeiboShare.Application.Features.Expenses.GetExpense;
using KakeiboShare.Application.Features.Expenses.ListExpenses;
using KakeiboShare.Application.Features.Expenses.UpdateExpense;
using KakeiboShare.Application.Features.Groups.CreateGroup;
using KakeiboShare.Application.Features.Groups.DeleteGroup;
using KakeiboShare.Application.Features.Groups.GetGroup;
using KakeiboShare.Application.Features.Groups.JoinByInvite;
using KakeiboShare.Application.Features.Groups.ListMembers;
using KakeiboShare.Application.Features.Groups.ListMyGroups;
using KakeiboShare.Application.Features.Groups.RegenerateInvite;
using KakeiboShare.Application.Features.Groups.RenameGroup;
using KakeiboShare.Application.Features.Settlements.CalculateSettlement;
using KakeiboShare.Application.Features.Settlements.CompleteSettlement;
using KakeiboShare.Application.Features.Settlements.ListSettlements;
using Microsoft.Extensions.DependencyInjection;

namespace KakeiboShare.Application;

/// <summary>
/// Application 層の DI 登録。全ユースケース Handler をスコープ付きで追加する。
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 全 Feature Handler をサービスコレクションに登録する。
    /// </summary>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<SignUpHandler>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<LogoutHandler>();

        services.AddScoped<CreateGroupHandler>();
        services.AddScoped<ListMyGroupsHandler>();
        services.AddScoped<GetGroupHandler>();
        services.AddScoped<RenameGroupHandler>();
        services.AddScoped<DeleteGroupHandler>();
        services.AddScoped<ListMembersHandler>();
        services.AddScoped<JoinByInviteHandler>();
        services.AddScoped<RegenerateInviteHandler>();

        services.AddScoped<CreateExpenseHandler>();
        services.AddScoped<ListExpensesHandler>();
        services.AddScoped<GetExpenseHandler>();
        services.AddScoped<UpdateExpenseHandler>();
        services.AddScoped<DeleteExpenseHandler>();

        services.AddScoped<CalculateSettlementHandler>();
        services.AddScoped<CompleteSettlementHandler>();
        services.AddScoped<ListSettlementsHandler>();

        return services;
    }
}
