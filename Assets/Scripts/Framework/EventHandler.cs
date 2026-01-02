using System;

namespace CDA.Framework.Interface
{
    [CDASingleton(0)]
    public class EventHandler : Singleton<EventHandler>, IDisposable
    {
        public void Dispose()
        {
            // TODO 在此释放托管资源
        }
    }
}