namespace Artimora.Kronos;

public readonly struct RequestReturnData
{
    public readonly byte[] Data;
    public readonly string Type;
    public readonly int StatusCode = 200;

    internal RequestReturnData(byte[] data, string type, int statusCode)
    {
        Data = data;
        Type = type;
        StatusCode = statusCode;
    }
}