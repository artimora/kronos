using System.Collections.Specialized;
using System.Net;
using System.Text.Json;

namespace Artimora.Kronos.Data;

public readonly struct RequestData(
    string userAgent,
    Dictionary<string, string>? bodyMultiPartData,
    CookieCollection cookies,
    NameValueCollection headers,
    Uri? requestUrl,
    string? requestRawUrl,
    string? bodyTextContents,
    Dictionary<string, string>? urlDynamicValues
)
{
    public readonly string UserAgent = userAgent;
    public readonly Dictionary<string, string>? BodyMultiPartData = bodyMultiPartData;
    public readonly CookieCollection Cookies = cookies;
    public readonly NameValueCollection Headers = headers;
    public readonly Uri? RequestUrl = requestUrl;
    public readonly string? RequestRawUrl = requestRawUrl;
    public readonly string? BodyTextContents = bodyTextContents;
    private readonly Dictionary<string, string> UrlDynamicValues = urlDynamicValues ?? [];

    public string GetParam(string paramName)
    {
        return UrlDynamicValues[paramName] ?? string.Empty;
    }

    public RequestReturnData Text(string text) => new(text, Server.GetReturnType(ReturnType.Text));

    public RequestReturnData Json(object data) => new(JsonSerializer.Serialize(data), Server.GetReturnType(ReturnType.Json));

    public RequestReturnData Html(string text) => new(text, Server.GetReturnType(ReturnType.Html));
}