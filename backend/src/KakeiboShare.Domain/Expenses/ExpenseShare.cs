namespace KakeiboShare.Domain.Expenses;

/// <summary>支出における1メンバーの負担。金額は整数（円）。</summary>
public readonly record struct ExpenseShare(Guid UserId, int Amount);
