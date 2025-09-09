using System.Net;
using CopperDevs.Logger;
using HttpMultipartParser;

namespace CopperDevs.Kronos;

public static class Util
{
    public static (Dictionary<string, string>?, string?) GetRequestBodyContents(HttpListenerRequest request)
    {
        var contents = (new Dictionary<string, string>(), string.Empty);
        using var body = CloneStream(request.InputStream);

        if (!request.HasEntityBody)
            return contents;

        try
        {
            var parser = MultipartFormDataParser.Parse(body);
            contents.Item1 = parser.Parameters.ToDictionary(p => p.Name, p => p.Data);
        }
        catch (Exception)
        {
            contents.Item1 = [];
        }

        try
        {
            using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
            var content = reader.ReadToEnd();
            contents.Item2 = content;
        }
        catch (Exception)
        {
            contents.Item2 = string.Empty;
        }

        return contents;
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
}