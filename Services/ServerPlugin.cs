using BootGen;

namespace Editor.Services
{
    public class ServerPlugin : IPlugin<ServerConfig> {
        public VirtualDisk Templates { get; set; }
        public VirtualDisk Files { get; set; }
        public ServerConfig Config { get; set; }
    }
}
