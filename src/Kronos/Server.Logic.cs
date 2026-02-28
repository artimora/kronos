using System.Net;
using System.Text;
using CopperDevs.Celesium;

namespace Artimora.Kronos;

public partial class Server
{
    private int port = 3000;

    // highkey the main reason a Shutdown and shouldRun duo is used here is that i have zero idea how CancellationToken works
    private bool shouldRun = true;
    public void Shutdown() => shouldRun = false;

    public async Task Listen() => await Listen(port);

    public async Task Listen(int targetPort)
    {
        port = targetPort;

        var uris = new string[2];
        uris[0] = $"http://127.0.0.1:{port}/";
        uris[1] = $"http://localhost:{port}/";

        await Listen(uris);
    }

    public async Task Listen(params string[] baseUris)
    {
        listener = new HttpListener();

        foreach (var uri in baseUris)
            listener.Prefixes.Add(uri);

        listener.Start();

        Log.Network(baseUris.AddFirstItem("Listening for connections in the following places:"));
        Console.WriteLine();

        try
        {
            await HandleIncomingConnections();
        }
        finally
        {
            listener.Close();
        }
    }

    private async Task HandleIncomingConnections()
    {
        while (shouldRun)
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

                var data = handled.Value.Data;
                resp.ContentType = handled.Value.Type;
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;
                resp.StatusCode = handled.Value.StatusCode;

                resp.AddHeader("X-Content-Type-Options", "nosniff");

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