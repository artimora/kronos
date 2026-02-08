namespace Artimora.Kronos;

public partial class Server
{
    public class Builder
    {
        private readonly Dictionary<string, Dictionary<RequestMethod, UserRequestMethod>> paths = [];

        public Dictionary<RequestMethod, UserRequestMethod> this[string path]
        {
            get
            {
                if (!paths.ContainsKey(path))
                    paths[path] = [];

                return paths[path];
            }
            set => paths[path] = value;
        }

        public Server Build()
        {
            var instance = new Server();

            foreach (var (path, value) in paths)
            {
                foreach (var (key, handler) in value)
                {
                    var method = key.Method;

                    instance.AddRequestMethod(path, handler, method);
                }
            }

            return instance;
        }
    }
}