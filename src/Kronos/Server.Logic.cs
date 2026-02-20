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
                    resp.Close();
                    continue;
                }


                var data = Encoding.UTF8.GetBytes(handled.Value.Data);
                resp.ContentType = handled.Value.Type;
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;
                resp.StatusCode = handled.Value.StatusCode;

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
        Log.Network($"Request: {method.ToUpper()} {path}");
        var forcedStatusCode = -1;

        var handlerRequests404 = false;

        if (!requestHandlers.TryGetValue(method, out var methodHandlers))
        {
            Log.Error($"No request handlers registered for method '{method.ToUpper()}'. Attempting /404 redirect.");
            handlerRequests404 = true;
        }

        var skipMethodHandlerMatching = false;
        UserRequestMethod handler = null!;

        if (handlerRequests404)
        {
            if (!Attempt404Redirect())
            {
                Console.WriteLine();
                return null!;
            }

            skipMethodHandlerMatching = true;
        }

        if (Util.FindMatchingTemplate(methodHandlers!.Keys, path, out var urlValues) is { } handlerMatch && !skipMethodHandlerMatching)
        {
            Log.Debug($"Match: {handlerMatch}");
            foreach (var kv in urlValues)
                Log.Debug($"  {kv.Key} = {kv.Value}");

            handler = methodHandlers[handlerMatch];
        }
        else if (skipMethodHandlerMatching && methodHandlers!.TryGetValue(path, out var foundHandler))
        {
            handler = foundHandler;
        }
        else
        {
            Log.Error($"No request handler found for path '{method.ToUpper()}' with method '{path}'. Attempting /404 redirect.");

            if (!Attempt404Redirect())
            {
                Console.WriteLine();
                return null!;
            }
        }

        Log.Info($"Request handler found: {method.ToUpper()} {path}");

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

        if (forcedStatusCode != -1)
            data = new RequestReturnData(data.Data, data.Type, forcedStatusCode);

        Log.Success($"Handled: {method.ToUpper()} {path}");

        Console.WriteLine();
        return data;

        bool Attempt404Redirect()
        {
            if (!requestHandlers.TryGetValue("GET", out var getHandlers) || !getHandlers.TryGetValue("/404", out var routeHandler))
            {
                Log.Error("Unable to retrieve /404 route handler");
                return false;
            }

            Log.Success("Redirected to 404 handler");
            handler = routeHandler;
            forcedStatusCode = 404;

            return true;
        }
    }
}