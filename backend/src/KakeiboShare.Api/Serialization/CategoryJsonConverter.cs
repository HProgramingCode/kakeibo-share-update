using System.Text.Json;
using System.Text.Json.Serialization;
using KakeiboShare.Domain.Expenses;

namespace KakeiboShare.Api.Serialization;

/// <summary>
/// Category enum と API 文字列（食費/日用品…）の相互変換。
/// </summary>
public sealed class CategoryJsonConverter : JsonConverter<Category>
{
    private static readonly Dictionary<string, Category> FromApi = new(StringComparer.Ordinal)
    {
        ["食費"] = Category.Food,
        ["日用品"] = Category.DailyGoods,
        ["光熱費"] = Category.Utilities,
        ["交通"] = Category.Transport,
        ["娯楽"] = Category.Entertainment,
        ["その他"] = Category.Other,
    };

    private static readonly Dictionary<Category, string> ToApi = FromApi
        .ToDictionary(x => x.Value, x => x.Key);

    /// <inheritdoc />
    public override Category Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is not null && FromApi.TryGetValue(value, out var category))
            return category;

        throw new JsonException($"未対応のカテゴリです: {value}");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, Category value, JsonSerializerOptions options)
    {
        if (!ToApi.TryGetValue(value, out var apiValue))
            throw new JsonException($"未対応のカテゴリです: {value}");

        writer.WriteStringValue(apiValue);
    }
}
