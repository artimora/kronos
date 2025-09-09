namespace CopperDevs.Kronos.Data;

internal readonly struct RequestReturnData(string data, string type)
{
    public readonly string Data = data;
    public readonly string Type = type;
}
