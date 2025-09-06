using CopperDevs.Kronos.Data;

namespace CopperDevs.Kronos;

public partial class Server
{
    private void AddRequestMethod(string path, Func<string> response, RequestMethod method, ReturnType returnType)
    {
        if (requestHandlers.TryGetValue(method, out var handler))
        {
            handler[path] = new Tuple<Func<string>, ReturnType>(response, returnType);
        }
        else
        {
            handler = new Dictionary<string, Tuple<Func<string>, ReturnType>>
            {
                [path] = new(response, returnType)
            };
        }

        requestHandlers[method] = handler;
    }

    public Server Get(string path, Func<string> response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Get, returnType);
        return this;
    }

    public Server Post(string path, Func<string> response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Post, returnType);
        return this;
    }

    public Server Patch(string path, Func<string> response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Patch, returnType);
        return this;
    }

    public Server Put(string path, Func<string> response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Put, returnType);
        return this;
    }

    public Server Delete(string path, Func<string> response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Delete, returnType);
        return this;
    }
}