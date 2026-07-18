using KakeiboShare.Api.Extensions;
using KakeiboShare.Application.Features.Settlements.CalculateSettlement;
using KakeiboShare.Application.Features.Settlements.CompleteSettlement;
using KakeiboShare.Application.Features.Settlements.ListSettlements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KakeiboShare.Api.Controllers;

/// <summary>
/// 精算 API。
/// </summary>
[ApiController]
[Route("api/groups/{groupId:guid}/settlements")]
[Authorize]
public sealed class SettlementsController(
    CalculateSettlementHandler calculateSettlement,
    CompleteSettlementHandler completeSettlement,
    ListSettlementsHandler listSettlements) : ControllerBase
{
    /// <summary>
    /// 精算プレビューを計算する（DB 保存しない）。
    /// </summary>
    [HttpPost("calculate")]
    public async Task<ActionResult<CalculateSettlementResponse>> Calculate(
        Guid groupId,
        CancellationToken cancellationToken) =>
        Ok(await calculateSettlement.HandleAsync(groupId, User.GetUserId(), cancellationToken));

    /// <summary>
    /// 精算を確定する。
    /// </summary>
    [HttpPost("complete")]
    [ProducesResponseType(typeof(CompleteSettlementResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CompleteSettlementResponse>> Complete(
        Guid groupId,
        CancellationToken cancellationToken)
    {
        var result = await completeSettlement.HandleAsync(groupId, User.GetUserId(), cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>
    /// 精算履歴を返す。
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ListSettlementsItemResponse>>> List(
        Guid groupId,
        CancellationToken cancellationToken) =>
        Ok(await listSettlements.HandleAsync(groupId, User.GetUserId(), cancellationToken));
}
