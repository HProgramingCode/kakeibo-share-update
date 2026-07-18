using KakeiboShare.Application.Features.Groups.CreateGroup;
using KakeiboShare.Application.Tests.Fakes;

namespace KakeiboShare.Application.Tests.Features.Groups;

public class CreateGroupHandlerTests
{
    [Fact]
    public async Task HandleAsync_グループが保存される()
    {
        var groups = new FakeGroupRepository();
        var uow = new FakeUnitOfWork();
        var userId = Guid.NewGuid();

        var handler = new CreateGroupHandler(groups, uow);
        var response = await handler.HandleAsync(new CreateGroupRequest("家族"), userId);

        Assert.Equal("家族", response.Name);
        Assert.False(string.IsNullOrWhiteSpace(response.InviteCode));
        Assert.Equal(1, uow.SaveCount);

        var saved = await groups.GetByIdAsync(response.Id);
        Assert.NotNull(saved);
        Assert.Contains(userId, saved!.MemberUserIds);
    }
}
