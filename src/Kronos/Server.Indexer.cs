using System;

namespace Artimora.Kronos;

public partial class Server
{
    public ServerMethodIndexer iGet = new();

    public class ServerMethodIndexer
    {
        internal ServerMethodIndexer()
        {

        }

        public struct SetterMethod
        {
            
        }
        
        
        public UserRequestMethod this[string id]
        {
            set => Set(id, value);
        }
    }
}
