using System.Net;

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

    private Server()
    {
    }
}