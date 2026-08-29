namespace Artimora.Kronos;

public readonly struct RequestReturnData
{
    public readonly byte[] Data;
    public readonly string? Type;
    public readonly int StatusCode = 200;
    public readonly Dictionary<string, string> Headers;

    internal RequestReturnData(byte[] data, string? type, int statusCode, Dictionary<string, string> headers)
    {
        Data = data;
        Type = type;
        StatusCode = statusCode;
        Headers = headers;
    }
}