namespace Artimora.Kronos;

public partial class Server
{
    internal void AddRequestMethod(string path, UserRequestMethod response, string method)
    {
        if (requestHandlers.TryGetValue(method, out var handler))
        {
            handler[path] = response;
        }
        else
        {
            handler = new Dictionary<string, UserRequestMethod>
            {
                [path] = response
            };
        }

        requestHandlers[method] = handler;
    }

    public Dictionary<GeneralRequestMethod, UserRequestMethod> this[string path]
    {
        get => [];
        set
        {
            foreach (var pair in value)
            {
                AddRequestMethod(path, pair.Value, pair.Key.Method);
            }
        }
    }
}