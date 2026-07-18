using KakeiboShare.Application.Common.Auth;
using KakeiboShare.Application.Common.Exceptions;
using KakeiboShare.Application.Features.Auth.Login;
using KakeiboShare.Application.Features.Auth.SignUp;
using KakeiboShare.Application.Tests.Fakes;
using KakeiboShare.Domain.Users;

namespace KakeiboShare.Application.Tests.Features.Auth;

public class SignUpHandlerTests
{
    [Fact]
    public async Task HandleAsync_新規登録_ユーザーが保存されトークンが返る()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var jwt = new FakeJwtTokenService();
        var uow = new FakeUnitOfWork();

        var handler = new SignUpHandler(users, hasher, jwt, uow);
        var response = await handler.HandleAsync(
            new SignUpRequest("test@example.com", "pass1234", "テスト"));

        Assert.Equal("fake-token", response.Token);
        Assert.Equal("test@example.com", response.User.Email);
        Assert.Equal(1, uow.SaveCount);
        Assert.Single(users.AllUsers);
    }

    [Fact]
    public async Task HandleAsync_Email重複_ConflictException()
    {
        var users = new FakeUserRepository();
        var existing = User.Create("test@example.com", "既存", "hash");
        users.Seed(existing);

        var handler = new SignUpHandler(users, new FakePasswordHasher(), new FakeJwtTokenService(), new FakeUnitOfWork());

        await Assert.ThrowsAsync<ConflictException>(() => handler.HandleAsync(
            new SignUpRequest("test@example.com", "pass1234", "テスト")));
    }
}

public class LoginHandlerTests
{
    [Fact]
    public async Task HandleAsync_正しい資格情報_トークンが返る()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Create("test@example.com", "テスト", hasher.HashPassword("pass1234"));
        users.Seed(user);

        var handler = new LoginHandler(users, hasher, new FakeJwtTokenService());
        var response = await handler.HandleAsync(new LoginRequest("test@example.com", "pass1234"));

        Assert.Equal("fake-token", response.Token);
        Assert.Equal(user.Id, response.User.Id);
    }

    [Fact]
    public async Task HandleAsync_誤パスワード_UnauthorizedException()
    {
        var users = new FakeUserRepository();
        var hasher = new FakePasswordHasher();
        var user = User.Create("test@example.com", "テスト", hasher.HashPassword("pass1234"));
        users.Seed(user);

        var handler = new LoginHandler(users, hasher, new FakeJwtTokenService());

        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.HandleAsync(
            new LoginRequest("test@example.com", "wrong")));
    }
}

internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => $"hash:{password}";

    public bool VerifyPassword(string hashedPassword, string password) =>
        hashedPassword == HashPassword(password);
}

internal sealed class FakeJwtTokenService : IJwtTokenService
{
    public string GenerateToken(User user) => "fake-token";
}
