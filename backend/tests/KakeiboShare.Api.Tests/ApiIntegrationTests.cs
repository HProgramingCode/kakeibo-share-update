using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace KakeiboShare.Api.Tests;

public class ApiIntegrationTests(KakeiboShareWebApplicationFactory factory) : IClassFixture<KakeiboShareWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_認証不要_200()
    {
        var response = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetGroups_未認証_401()
    {
        var response = await _client.GetAsync("/api/groups");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_新規登録_201とトークン()
    {
        var email = $"test-{Guid.NewGuid():N}@example.com";
        var response = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            email,
            password = "pass1234",
            name = "テスト",
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrWhiteSpace(token.GetString()));
    }

    [Fact]
    public async Task SignUp後_Groups作成_201()
    {
        var email = $"test-{Guid.NewGuid():N}@example.com";
        var signUp = await _client.PostAsJsonAsync("/api/auth/signup", new
        {
            email,
            password = "pass1234",
            name = "テスト",
        });
        var auth = await signUp.Content.ReadFromJsonAsync<JsonElement>();
        var token = auth.GetProperty("token").GetString()!;

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/groups");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { name = "家族" });

        var response = await _client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task SignUp_Email重複_409()
    {
        var email = $"dup-{Guid.NewGuid():N}@example.com";
        var body = new { email, password = "pass1234", name = "テスト" };

        var first = await _client.PostAsJsonAsync("/api/auth/signup", body);
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);

        var second = await _client.PostAsJsonAsync("/api/auth/signup", body);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var error = await second.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("CONFLICT", error.GetProperty("error").GetProperty("code").GetString());
    }
}
