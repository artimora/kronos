using System.Collections.Specialized;
using System.Net;

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
}