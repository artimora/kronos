using System.Net;
using HttpMultipartParser;

namespace CopperDevs.Kronos;

public static class Util
{
    public static Dictionary<string, string> GetRequestPostData(HttpListenerRequest request)
    {
        if (!request.HasEntityBody)
            return new Dictionary<string, string>();

        using var body = request.InputStream;

        // Parse form-data
        var parser = MultipartFormDataParser.Parse(body);

        // Convert to dictionary (name => value)
        return parser.Parameters.ToDictionary(p => p.Name, p => p.Data);
    }
}