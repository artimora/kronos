namespace Artimora.Kronos.Data;

public readonly struct RequestReturnData
{
    public readonly string Data;
    public readonly string Type;

    internal RequestReturnData(string data, string type)
    {
        Data = data;
        Type = type;
    }
}