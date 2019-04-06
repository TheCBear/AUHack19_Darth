using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSyncronizer
{
    public class InstagramImageSyncronizer:IImageSyncronizer
    {
        public IFilter Filter { get; }
        public IFetcher Fetcher { get; }
        public IWatcher Watcher { get; }
        public void RunImageSyncing()
        {
            
        }
    }
}
