using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace EasyLauncher
{
    public interface IConsoleHandler
    {
        void AddStopHandler(Action action);
        void RemoveAll();
    }

    public sealed class ConsoleHandler : IConsoleHandler
    {
        private readonly ConcurrentDictionary<HandlerRoutine, object> actions = new ConcurrentDictionary<HandlerRoutine, object>();  

        public void AddStopHandler(Action action)
        {
            HandlerRoutine handler = dwCtrlType => ConsoleCtrlHandler(dwCtrlType, action);
            actions.TryAdd(handler, null);
            SetConsoleCtrlHandler(handler, true);
        }

        public void RemoveAll()
        {
            foreach (var handler in actions.Keys)
            {
                SetConsoleCtrlHandler(handler, false);
            }
        }

        private static bool ConsoleCtrlHandler(uint dwCtrlType, Action action)
        {
            try
            {
                switch (dwCtrlType)
                {
                    case CTRL_C_EVENT:
                    case CTRL_BREAK_EVENT:
                    case CTRL_CLOSE_EVENT:
                    case CTRL_SHUTDOWN_EVENT:
                        action();
                        break;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        private const UInt32 CTRL_BREAK_EVENT = 1;
        private const UInt32 CTRL_C_EVENT = 0;
        private const UInt32 CTRL_CLOSE_EVENT = 2;
        private const UInt32 CTRL_SHUTDOWN_EVENT = 6;

        private delegate bool HandlerRoutine(uint dwCtrlType);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);
    }
}