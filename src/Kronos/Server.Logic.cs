using System.Net;
using System.Text;
using System.Text.Json;
using CopperDevs.Kronos.Data;
using CopperDevs.Logger;

namespace CopperDevs.Kronos;

public partial class Server
{
    public void Listen(int port)
    {
        listener = new HttpListener();
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Start();

        Log.Network($"Listening for connections on http://127.0.0.1:{port}/");
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
                var handled = HandleRequest(GetMethod(req.HttpMethod), req.Url!.LocalPath, req);

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
                resp.ContentType = "application/json";

                var message = ReturnMessage<HttpStatusCode>.CreateWithError(e.Message).ToJson();
                Log.Debug(message);
                var data = Encoding.UTF8.GetBytes(message);
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data);
                resp.Close();
            }
        }
    }

    private RequestReturnData? HandleRequest(RequestMethod method, string path, HttpListenerRequest request)
    {

        Log.Network($"Request: {method.ToString().ToUpper()} {path}");

        if (!requestHandlers.TryGetValue(method, out var methodHandlers))
        {
            Log.Error($"No request handlers registered for method '{method.ToString().ToUpper()}'.");

            Console.WriteLine();
            return null!;
        }

        UserRequestMethodData handler;

        if (Util.FindMatchingTemplate(methodHandlers.Keys, path, out var urlValues) is string handlerMatch)
        {
            Log.Debug($"Match: {handlerMatch}");
            foreach (var kv in urlValues)
                Log.Debug($"  {kv.Key} = {kv.Value}");

            handler = methodHandlers[handlerMatch];
        }

        if (methodHandlers.TryGetValue(path, out var foundHandler))
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

        var (FormData, RawBody) = Util.GetRequestBodyContents(request);

        var requestData = new RequestData
        (
            request.UserAgent,
            FormData,
            request.Cookies,
            request.Headers,
            request.Url,
            request.RawUrl,
            RawBody,
            urlValues ?? []
        );

        var data = handler.Item1(requestData);
        var type = GetReturnType(handler.Item2);

        Log.Success($"Handled: {method.ToString().ToUpper()} {path}");

        Console.WriteLine();
        return new RequestReturnData(data, type);
    }
}