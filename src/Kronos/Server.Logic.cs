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
                // Log.Debug($"\n{JsonSerializer.Serialize(Util.GetRequestPostData(req))}"); // TODO: have this data be accessible in the client per method shi 
                var handled = HandleRequest(GetMethod(req.HttpMethod), req.Url!.LocalPath);

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
                resp.ContentType = "text/plain";
                
                var data = Encoding.UTF8.GetBytes(e.Message);
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;
                
                await resp.OutputStream.WriteAsync(data);
                resp.Close();
            }
        }
    }

    private RequestReturnData? HandleRequest(RequestMethod method, string path)
    {
        Log.Network($"Request: {method.ToString().ToUpper()} {path}");

        if (!requestHandlers.TryGetValue(method, out var methodHandlers))
        {
            Log.Error($"No request handlers registered for method '{method.ToString().ToUpper()}'.");

            Console.WriteLine();
            return null!;
        }

        if (!methodHandlers.TryGetValue(path, out var handler))
        {
            Log.Error($"No request handler found for path '{method.ToString().ToUpper()}' with method '{path}'.");

            Console.WriteLine();
            return null!;
        }

        Log.Info($"Request handler found: {method.ToString().ToUpper()} {path}");

        var data = handler.Item1();
        var type = GetReturnType(handler.Item2);

        Log.Success($"Handled: {method.ToString().ToUpper()} {path}");

        Console.WriteLine();
        return new RequestReturnData(data, type);
    }
}