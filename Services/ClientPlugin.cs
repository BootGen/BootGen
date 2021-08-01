using BootGen;

namespace Editor.Services
{
    public class ClientPlugin : IPlugin<ClientConfig> {
        public VirtualDisk Templates { get; set; }
        public VirtualDisk Files { get; set; }
        public ClientConfig Config { get; set; }
    }
}
