using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CopperDevs.Kronos.Data;

public struct ReturnMessage<T>
{
    [JsonPropertyName("isError")]
    public bool IsError;
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage;

    [JsonPropertyName("message")]
    public T? Data;

    public ReturnMessage() {}

    public static ReturnMessage<T> CreateWithData(T data)
    {
        return new ReturnMessage<T>() {
            IsError = false,
            ErrorMessage = null,
            Data = data
        };
    }

    public static ReturnMessage<T> CreateWithError(string message){
        return new ReturnMessage<T>(){
            IsError = true,
            ErrorMessage = message,
            Data = null
        };
    }

    public override string ToString() {
        return JsonSerializer.SerializeObject(this);
    }
}
