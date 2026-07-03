var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Phase 1 は骨組みのみ。ヘルスチェックだけを提供する（認証・DI・EF は Phase 2 以降）。
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

// 統合テスト（Api.Tests / WebApplicationFactory）からエントリポイントを参照できるように公開する。
public partial class Program { }
