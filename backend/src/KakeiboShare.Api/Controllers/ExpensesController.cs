using KakeiboShare.Api.Extensions;
using KakeiboShare.Application.Features.Expenses.CreateExpense;
using KakeiboShare.Application.Features.Expenses.DeleteExpense;
using KakeiboShare.Application.Features.Expenses.GetExpense;
using KakeiboShare.Application.Features.Expenses.ListExpenses;
using KakeiboShare.Application.Features.Expenses.UpdateExpense;
using KakeiboShare.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KakeiboShare.Api.Controllers;

/// <summary>
/// 支出 API。
/// </summary>
[ApiController]
[Authorize]
public sealed class ExpensesController(
    CreateExpenseHandler createExpense,
    ListExpensesHandler listExpenses,
    GetExpenseHandler getExpense,
    UpdateExpenseHandler updateExpense,
    DeleteExpenseHandler deleteExpense) : ControllerBase
{
    /// <summary>
    /// 支出を登録する。
    /// </summary>
    [HttpPost("api/groups/{groupId:guid}/expenses")]
    [ProducesResponseType(typeof(CreateExpenseResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateExpenseResponse>> Create(
        Guid groupId,
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createExpense.HandleAsync(groupId, request, User.GetUserId(), cancellationToken);
        return CreatedAtAction(nameof(Get), new { expenseId = result.Id }, result);
    }

    /// <summary>
    /// グループの支出一覧を返す。
    /// </summary>
    [HttpGet("api/groups/{groupId:guid}/expenses")]
    public async Task<ActionResult<IReadOnlyList<ListExpensesItemResponse>>> List(
        Guid groupId,
        [FromQuery] string? settled,
        CancellationToken cancellationToken) =>
        Ok(await listExpenses.HandleAsync(groupId, ParseSettled(settled), User.GetUserId(), cancellationToken));

    /// <summary>
    /// 支出詳細を返す。
    /// </summary>
    [HttpGet("api/expenses/{expenseId:guid}")]
    public async Task<ActionResult<GetExpenseResponse>> Get(Guid expenseId, CancellationToken cancellationToken) =>
        Ok(await getExpense.HandleAsync(expenseId, User.GetUserId(), cancellationToken));

    /// <summary>
    /// 支出を更新する（未精算のみ）。
    /// </summary>
    [HttpPut("api/expenses/{expenseId:guid}")]
    public async Task<ActionResult<CreateExpenseResponse>> Update(
        Guid expenseId,
        [FromBody] UpdateExpenseRequest request,
        CancellationToken cancellationToken) =>
        Ok(await updateExpense.HandleAsync(expenseId, request, User.GetUserId(), cancellationToken));

    /// <summary>
    /// 支出を削除する（未精算のみ）。
    /// </summary>
    [HttpDelete("api/expenses/{expenseId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid expenseId, CancellationToken cancellationToken)
    {
        await deleteExpense.HandleAsync(expenseId, User.GetUserId(), cancellationToken);
        return NoContent();
    }

    private static bool? ParseSettled(string? settled) => settled?.ToLowerInvariant() switch
    {
        "true" => true,
        "false" => false,
        "all" or null or "" => null,
        _ => throw new DomainException("settled は true / false / all のいずれかを指定してください"),
    };
}
