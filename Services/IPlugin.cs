using BootGen;

namespace Editor.Services
{
    public interface IPlugin<T> {
        VirtualDisk Templates { get; set; }
        VirtualDisk Files { get; set; }
        T Config { get; set; }
    }
}
