using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Application.Common.Models;

/// <summary>
/// 支出詳細取得用の読み取りモデル。Description / ExpenseDate は Domain 未保持のためここで保持する。
/// </summary>
public sealed record ExpenseDetail(
    Expense Expense,
    string Description,
    DateOnly ExpenseDate,
    IReadOnlyDictionary<Guid, int?> ShareRatios);

/// <summary>
/// 支出一覧 API 用のサマリ読み取りモデル。
/// </summary>
public sealed record ExpenseSummary(
    Guid Id,
    int Amount,
    Guid PaidByUserId,
    Category Category,
    DateOnly ExpenseDate,
    bool Settled);

/// <summary>
/// グループメンバー一覧用の読み取りモデル。
/// </summary>
public sealed record GroupMemberDetail(
    Guid UserId,
    string Name,
    DateTimeOffset JoinedAt);

/// <summary>
/// 割り勘入力の1メンバー分。
/// </summary>
public sealed record ShareInput(Guid UserId, int? Ratio);
