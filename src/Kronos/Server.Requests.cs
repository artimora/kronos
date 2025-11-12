using Artimora.Kronos.Data;

namespace Artimora.Kronos;

public partial class Server
{
    private void AddRequestMethod(string path, UserRequestMethod response, RequestMethod method, ReturnType returnType)
    {
        if (requestHandlers.TryGetValue(method, out var handler))
        {
            handler[path] = new UserRequestMethodData(response, returnType);
        }
        else
        {
            handler = new Dictionary<string, UserRequestMethodData>
            {
                [path] = new(response, returnType)
            };
        }

        requestHandlers[method] = handler;
    }

    public Server Get(string path, UserRequestMethod response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Get, returnType);
        return this;
    }

    public Server Get(string path, UserRequestMethodDataless response, ReturnType returnType)
    {
        AddRequestMethod(path, _ => response(), RequestMethod.Get, returnType);
        return this;
    }

    public Server Post(string path, UserRequestMethod response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Post, returnType);
        return this;
    }

    public Server Post(string path, UserRequestMethodDataless response, ReturnType returnType)
    {
        AddRequestMethod(path, _ => response(), RequestMethod.Post, returnType);
        return this;
    }

    public Server Patch(string path, UserRequestMethod response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Patch, returnType);
        return this;
    }

    public Server Patch(string path, UserRequestMethodDataless response, ReturnType returnType)
    {
        AddRequestMethod(path, _ => response(), RequestMethod.Patch, returnType);
        return this;
    }

    public Server Put(string path, UserRequestMethod response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Put, returnType);
        return this;
    }

    public Server Put(string path, UserRequestMethodDataless response, ReturnType returnType)
    {
        AddRequestMethod(path, _ => response(), RequestMethod.Put, returnType);
        return this;
    }

    public Server Delete(string path, UserRequestMethod response, ReturnType returnType)
    {
        AddRequestMethod(path, response, RequestMethod.Delete, returnType);
        return this;
    }

    public Server Delete(string path, UserRequestMethodDataless response, ReturnType returnType)
    {
        AddRequestMethod(path, _ => response(), RequestMethod.Delete, returnType);
        return this;
    }
}