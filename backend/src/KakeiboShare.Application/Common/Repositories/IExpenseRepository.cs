using KakeiboShare.Application.Common.Models;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Application.Common.Repositories;

/// <summary>
/// 支出集約の永続化。Shares は常に Include して Domain にマッピングする。
/// Description / ExpenseDate は Domain 未保持のため引数で受け渡す。
/// </summary>
public interface IExpenseRepository
{
    /// <summary>ID で支出と Shares を取得する。</summary>
    Task<Expense?> GetWithSharesAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>ID で支出詳細（Description / ExpenseDate / ShareRatio 含む）を取得する。</summary>
    Task<ExpenseDetail?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// グループの支出一覧。settled: true=精算済 / false=未精算 / null=全件。
    /// </summary>
    Task<IReadOnlyList<Expense>> ListByGroupAsync(Guid groupId, bool? settled, CancellationToken cancellationToken = default);

    /// <summary>グループの支出一覧サマリ（API 一覧用）。</summary>
    Task<IReadOnlyList<ExpenseSummary>> ListSummariesByGroupAsync(Guid groupId, bool? settled,
        CancellationToken cancellationToken = default);

    /// <summary>新規支出と Shares を追加する。shareRatios は Ratio 時のみ値を持つ。</summary>
    Task AddAsync(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, CancellationToken cancellationToken = default);

    /// <summary>未精算支出を更新する（Shares は全置換）。</summary>
    Task UpdateAsync(Expense expense, string description, DateOnly expenseDate,
        IReadOnlyDictionary<Guid, int?> shareRatios, CancellationToken cancellationToken = default);

    /// <summary>支出を削除する（Cascade で Shares も削除）。</summary>
    Task RemoveAsync(Expense expense, CancellationToken cancellationToken = default);
}
