using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Artimora.Kronos.Data;

public struct ReturnMessage<T>
{
    [JsonPropertyName("isError")]
    public bool IsError { get; set; }
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public ReturnMessage() { }

    public static ReturnMessage<T> CreateWithData(T data)
    {
        return new ReturnMessage<T>()
        {
            IsError = false,
            Data = data
        };
    }

    public static ReturnMessage<T> CreateWithError(string message)
    {
        return new ReturnMessage<T>()
        {
            IsError = true,
            ErrorMessage = message,
        };
    }

    [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed.")]
    // JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.
    public readonly string ToJson() => JsonSerializer.Serialize(this);
}
