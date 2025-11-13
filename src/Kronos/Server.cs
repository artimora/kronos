using System.Net;
using Artimora.Kronos.Data;

namespace Artimora.Kronos;

public partial class Server
{
    private HttpListener listener = null!;

    private readonly Dictionary<RequestMethod, Dictionary<string, UserRequestMethod>> requestHandlers = new();

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

    public Server()
    {
        Get = new ServerMethodIndexer(this, RequestMethod.Get);
        Post = new ServerMethodIndexer(this, RequestMethod.Post);
        Patch = new ServerMethodIndexer(this, RequestMethod.Patch);
        Put = new ServerMethodIndexer(this, RequestMethod.Put);
        Delete = new ServerMethodIndexer(this, RequestMethod.Delete);
    }
}