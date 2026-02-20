namespace Artimora.Kronos;

public readonly struct RequestReturnData
{
    public readonly string Data;
    public readonly string Type;
    public readonly int StatusCode = 200;

    internal RequestReturnData(string data, string type, int statusCode)
    {
        Data = data;
        Type = type;
        StatusCode = statusCode;
    }
}