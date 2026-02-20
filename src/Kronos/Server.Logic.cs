using System.Net;
using System.Text;
using CopperDevs.Logger;

namespace Artimora.Kronos;

public partial class Server
{
    private int port = 3000;

    public void Listen() => Listen(port);

    public void Listen(int targetPort)
    {
        port = targetPort;
        
        var uris = new string[2];
        uris[0] = $"http://127.0.0.1:{port}/";
        uris[1] = $"http://localhost:{port}/";

        Listen(uris);
    }


    public void Listen(params string[] baseUris)
    {
        listener = new HttpListener();

        foreach (var uris in baseUris)
            listener.Prefixes.Add(uris);

        listener.Start();

        Log.Network(baseUris.AddFirstItem($"Listening for connections in the following places:"));

        Console.WriteLine();

        // Handle requests
        var listenTask = HandleIncomingConnections();
        listenTask.GetAwaiter().GetResult();

        // Close the listener
        listener.Close();
    }

    private async Task HandleIncomingConnections()
    {
        while (true)
        {
            var ctx = await listener.GetContextAsync();

            var req = ctx.Request;
            var resp = ctx.Response;

            try
            {
                var handled = HandleRequest(req.HttpMethod, req.Url!.LocalPath, req);

                if (handled is null)
                {
                    resp.StatusCode = (int)HttpStatusCode.NotFound;
                    await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes([]));
                    resp.Close();
                    continue;
                }


                var data = Encoding.UTF8.GetBytes(handled.Value.Data);
                resp.ContentType = handled.Value.Type;
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                resp.AddHeader("X-Content-Type-Options", "nosniff");

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data);
                resp.Close();
            }
            catch (Exception e)
            {
                Log.Exception(e);

                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                resp.Close();
            }
        }
    }

    private RequestReturnData? HandleRequest(string method, string path, HttpListenerRequest request)
    {
        Log.Network($"Request: {method.ToString().ToUpper()} {path}");

        if (!requestHandlers.TryGetValue(method, out var methodHandlers))
        {
            Log.Error($"No request handlers registered for method '{method.ToString().ToUpper()}'.");

            Console.WriteLine();
            return null!;
        }

        UserRequestMethod handler = null!;

        if (Util.FindMatchingTemplate(methodHandlers.Keys, path, out var urlValues) is string handlerMatch)
        {
            Log.Debug($"Match: {handlerMatch}");
            foreach (var kv in urlValues)
                Log.Debug($"  {kv.Key} = {kv.Value}");

            handler = methodHandlers[handlerMatch];
        }
        else if (methodHandlers.TryGetValue(path, out var foundHandler))
        {
            if (foundHandler is not null)
                handler = foundHandler;
        }
        else
        {
            Log.Error($"No request handler found for path '{method.ToString().ToUpper()}' with method '{path}'.");
            Console.WriteLine();
            return null!;
        }

        if (handler is null)
            return null!;


        Log.Info($"Request handler found: {method.ToString().ToUpper()} {path}");

        var (formData, rawBody) = Util.GetRequestBodyContents(request);

        var requestData = new RequestData
        (
            request.UserAgent!,
            formData,
            request.Cookies,
            request.Headers,
            request.Url,
            request.RawUrl,
            rawBody,
            urlValues ?? []
        );

        var data = handler(requestData);


        Log.Success($"Handled: {method.ToString().ToUpper()} {path}");

        Console.WriteLine();
        return data;
    }
}