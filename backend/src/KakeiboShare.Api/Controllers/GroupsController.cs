using KakeiboShare.Api.Extensions;
using KakeiboShare.Application.Features.Groups.CreateGroup;
using KakeiboShare.Application.Features.Groups.DeleteGroup;
using KakeiboShare.Application.Features.Groups.GetGroup;
using KakeiboShare.Application.Features.Groups.JoinByInvite;
using KakeiboShare.Application.Features.Groups.ListMembers;
using KakeiboShare.Application.Features.Groups.ListMyGroups;
using KakeiboShare.Application.Features.Groups.RegenerateInvite;
using KakeiboShare.Application.Features.Groups.RenameGroup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KakeiboShare.Api.Controllers;

/// <summary>
/// グループ API。
/// </summary>
[ApiController]
[Route("api/groups")]
[Authorize]
public sealed class GroupsController(
    CreateGroupHandler createGroup,
    ListMyGroupsHandler listMyGroups,
    GetGroupHandler getGroup,
    RenameGroupHandler renameGroup,
    DeleteGroupHandler deleteGroup,
    ListMembersHandler listMembers,
    JoinByInviteHandler joinByInvite,
    RegenerateInviteHandler regenerateInvite) : ControllerBase
{
    /// <summary>
    /// グループを作成する。
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateGroupResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CreateGroupResponse>> Create(
        [FromBody] CreateGroupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createGroup.HandleAsync(request, User.GetUserId(), cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    /// <summary>
    /// 自分が所属するグループ一覧を返す。
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GroupListItemResponse>>> List(CancellationToken cancellationToken) =>
        Ok(await listMyGroups.HandleAsync(User.GetUserId(), cancellationToken));

    /// <summary>
    /// グループ詳細を返す。
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetGroupResponse>> Get(Guid id, CancellationToken cancellationToken) =>
        Ok(await getGroup.HandleAsync(id, User.GetUserId(), cancellationToken));

    /// <summary>
    /// グループ名を変更する。
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RenameGroupResponse>> Rename(
        Guid id,
        [FromBody] RenameGroupRequest request,
        CancellationToken cancellationToken) =>
        Ok(await renameGroup.HandleAsync(id, request, User.GetUserId(), cancellationToken));

    /// <summary>
    /// グループを削除する。
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await deleteGroup.HandleAsync(id, User.GetUserId(), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// メンバー一覧を返す。
    /// </summary>
    [HttpGet("{id:guid}/members")]
    public async Task<ActionResult<IReadOnlyList<ListMembersItemResponse>>> ListMembers(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await listMembers.HandleAsync(id, User.GetUserId(), cancellationToken));

    /// <summary>
    /// 招待コードでグループに参加する。
    /// </summary>
    [HttpPost("join")]
    public async Task<ActionResult<JoinByInviteResponse>> Join(
        [FromBody] JoinByInviteRequest request,
        CancellationToken cancellationToken) =>
        Ok(await joinByInvite.HandleAsync(request, User.GetUserId(), cancellationToken));

    /// <summary>
    /// 招待コードを再発行する。
    /// </summary>
    [HttpPost("{id:guid}/invite/regenerate")]
    public async Task<ActionResult<RegenerateInviteResponse>> RegenerateInvite(
        Guid id,
        CancellationToken cancellationToken) =>
        Ok(await regenerateInvite.HandleAsync(id, User.GetUserId(), cancellationToken));
}
