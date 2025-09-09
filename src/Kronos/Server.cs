using System.Net;
using CopperDevs.Kronos.Data;

namespace CopperDevs.Kronos;

public partial class Server
{
    private HttpListener listener = null!;

    private readonly Dictionary<RequestMethod, Dictionary<string, UserRequestMethodData>> requestHandlers = new();

    private static string GetReturnType(ReturnType returnType)
    {
        return returnType switch
        {
            ReturnType.Text => "text/plain",
            ReturnType.Json => "application/json",
            ReturnType.Html => "text/html",
            _ => throw new ArgumentOutOfRangeException(nameof(returnType), returnType, null)
        };
    }

    private static RequestMethod GetMethod(string method)
    {
        return method switch
        {
            "GET" => RequestMethod.Get,
            "POST" => RequestMethod.Post,
            "PUT" => RequestMethod.Put,
            "DELETE" => RequestMethod.Delete,
            "PATCH" => RequestMethod.Patch,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
    }
}