using System.Net;
using Artimora.Kronos.Data;

namespace Artimora.Kronos;

public partial class Server
{
    private HttpListener listener = null!;

    private readonly Dictionary<string, Dictionary<string, UserRequestMethod>> requestHandlers = new();

    internal static string GetReturnType(ReturnType returnType)
    {
        return returnType switch
        {
            ReturnType.Text => "text/plain",
            ReturnType.Json => "application/json",
            ReturnType.Html => "text/html",
            _ => throw new ArgumentOutOfRangeException(nameof(returnType), returnType, null)
        };
    }

    public static RequestMethod GetMethod(string method)
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

    public static string GetMethod(RequestMethod method)
    {
        return method switch
        {
            RequestMethod.Get => "GET",
            RequestMethod.Post => "POST",
            RequestMethod.Patch => "PATCH",
            RequestMethod.Put => "PUT",
            RequestMethod.Delete => "DELETE",
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
        };
    }

    internal Server()
    {
    }
}