using Artimora.Kronos.Data;

namespace Artimora.Kronos;

public partial class Server
{
    public ServerMethodIndexer Get;
    public ServerMethodIndexer Post;
    public ServerMethodIndexer Patch;
    public ServerMethodIndexer Put;
    public ServerMethodIndexer Delete;

    private void AddRequestMethod(string path, UserRequestMethod response, RequestMethod method)
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

    public class ServerMethodIndexer
    {
        private readonly Server server;
        private readonly RequestMethod method;

        internal ServerMethodIndexer(Server server, RequestMethod method)
        {
            this.server = server;
            this.method = method;
        }

        public UserRequestMethod this[string path]
        {
            set => server.AddRequestMethod(path, value, method);
        }
    }
}