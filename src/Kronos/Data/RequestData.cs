using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Artimora.Kronos;

public readonly struct RequestData(
    string userAgent,
    Dictionary<string, string>? bodyMultiPartData,
    CookieCollection cookies,
    NameValueCollection headers,
    Uri? requestUrl,
    string? requestRawUrl,
    string? bodyTextContents,
    Dictionary<string, string>? urlDynamicValues,
    string rawQuery
)
{
    public readonly string UserAgent = userAgent;
    public readonly Dictionary<string, string>? BodyMultiPartData = bodyMultiPartData;
    public readonly CookieCollection Cookies = cookies;
    public readonly NameValueCollection Headers = headers;
    public readonly Uri? RequestUrl = requestUrl;
    public readonly string? RequestRawUrl = requestRawUrl;
    public readonly string? BodyTextContents = bodyTextContents;
    public readonly string? RawQuery = rawQuery;

    // ReSharper disable once InconsistentNaming
    private readonly NameValueCollection queryValues = HttpUtility.ParseQueryString(rawQuery);
    private readonly Dictionary<string, string> urlDynamicValues = urlDynamicValues ?? [];

    public string GetParam(string paramName) => urlDynamicValues[paramName] ?? string.Empty;

    public string GetQueryParam(string paramName) => queryValues.Get(paramName) ?? string.Empty;

    public Dictionary<string, string> GetAllParams() => urlDynamicValues;

    public Dictionary<string, string> GetAllQueryParams()
    {
        var values = queryValues;
        return (queryValues.AllKeys as string[]).ToDictionary(item => item, values.Get)!; // some straight bs here btw
    }

#pragma warning disable CA1822
    // ReSharper disable MemberCanBeMadeStatic.Global
    // ReSharper disable MemberCanBePrivate.Global
    public RequestReturnData Body(string contents, string mime = "text/plain", int statusCode = 200) => new(Encoding.UTF8.GetBytes(contents), mime, statusCode);
    public RequestReturnData Body(byte[] contents, string mime = "application/octet-stream", int statusCode = 200) => new(contents, mime, statusCode);
    // ReSharper restore MemberCanBePrivate.Global
    // ReSharper restore MemberCanBeMadeStatic.Global
#pragma warning restore CA1822

    public RequestReturnData Text(string text, int statusCode = 200) => Body(text, "text/plain", statusCode);

#pragma warning disable IL2026, IL3050
    public RequestReturnData Json(object data, int statusCode = 200) => Body(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data)), "application/json", statusCode);
#pragma warning restore IL2026, IL3050

    public RequestReturnData Html(string text, int statusCode = 200) => Body(text, "text/html", statusCode);
}