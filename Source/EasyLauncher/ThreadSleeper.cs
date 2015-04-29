using System;
using System.Threading;

namespace EasyLauncher
{
    public interface IThreadSleeper
    {
        void Sleep(TimeSpan timeout);
    }

    public sealed class ThreadSleeper : IThreadSleeper
    {
        public void Sleep(TimeSpan timeout)
        {
            Thread.Sleep(timeout);
        }
    }
}