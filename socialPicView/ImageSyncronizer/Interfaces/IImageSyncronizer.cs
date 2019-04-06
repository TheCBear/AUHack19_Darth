using System.Runtime.CompilerServices;

namespace ImageSyncronizer
{
    public interface IImageSyncronizer
    {
        IFilter Filter { get; }
        IFetcher Fetcher { get; }
        IWatcher Watcher { get; }
        void RunImageSyncing();
    }
}