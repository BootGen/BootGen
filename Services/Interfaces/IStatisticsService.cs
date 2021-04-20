using BootGen;

namespace Editor.Services
{
    public interface IStatisticsService
    {
        void OnGenerated(DataModel model, string input, StatEvent statEvent);
    }
}
