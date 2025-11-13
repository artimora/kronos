using System.Net;
using CopperDevs.Logger;
using HttpMultipartParser;

namespace Artimora.Kronos;

public static class Util
{
    public static (Dictionary<string, string>? FormData, string? RawBody) GetRequestBodyContents(HttpListenerRequest request)
    {
        if (!request.HasEntityBody)
            return (new Dictionary<string, string>(), string.Empty);

        using var body = CloneStream(request.InputStream);

        var formData = new Dictionary<string, string>();
        string rawBody = string.Empty;

        try
        {
            var parser = MultipartFormDataParser.Parse(body);
            formData = parser.Parameters.ToDictionary(p => p.Name, p => p.Data);
        }
        catch (Exception ex)
        {
            Log.Debug($"Multipart parsing failed: {ex.Message}");
        }

        body.Position = 0;
        try
        {
            using var reader = new StreamReader(body, request.ContentEncoding, leaveOpen: true);
            rawBody = reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            Log.Debug($"Raw body read failed: {ex.Message}");
        }

        return (formData, rawBody);
    }

    public static MemoryStream CloneStream(Stream originalStream)
    {
        if (originalStream.CanSeek)
            originalStream.Position = 0;

        MemoryStream clonedStream = new();
        originalStream.CopyTo(clonedStream);
        clonedStream.Position = 0;
        return clonedStream;
    }

    public static bool TryMatchPath(
        string dynamicTemplate,
        string filledPath,
        out Dictionary<string, string> values)
    {
        values = [];

        var templateParts = dynamicTemplate.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var filledParts = filledPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (templateParts.Length != filledParts.Length)
            return false;

        for (int i = 0; i < templateParts.Length; i++)
        {
            var templatePart = templateParts[i];
            var filledPart = filledParts[i];

            if (templatePart.StartsWith(':'))
            {
                var key = templatePart[1..];
                values[key] = filledPart;
            }
            else if (!string.Equals(templatePart, filledPart, StringComparison.OrdinalIgnoreCase))
            {
                return false; // mismatch in static segment
            }
        }

        return true;
    }

    public static string? FindMatchingTemplate(
        IEnumerable<string> dynamicTemplates,
        string filledPath,
        out Dictionary<string, string> values)
    {
        foreach (var template in dynamicTemplates)
        {
            if (TryMatchPath(template, filledPath, out values))
                return template;
        }

        values = [];
        return null; // no match
    }
}