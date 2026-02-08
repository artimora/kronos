namespace Artimora.Kronos.Data;


public class GeneralRequestMethod(string method)
{
    public readonly string Method = method;

    public static implicit operator string(GeneralRequestMethod method) => method.Method;

    public static implicit operator GeneralRequestMethod(string method) => new(method);

    public static implicit operator GeneralRequestMethod(RequestMethod method) => new(Server.GetMethod(method));
}