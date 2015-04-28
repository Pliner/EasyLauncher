using System;
using System.Runtime.InteropServices;

namespace EasyLauncher
{
    public interface IConsoleHandler
    {
        void Set();
        void Unset();
    }

    public class ConsoleHandler : IConsoleHandler
    {
        private readonly Action onStop;
        private bool isSet;

        public ConsoleHandler(Action onStop)
        {
            this.onStop = onStop;
        }

        public void Set()
        {
            if (isSet)
                return;
            SetConsoleCtrlHandler(ConsoleCtrlHandler, true);
            isSet = true;
        }

        public void Unset()
        {
            if (!isSet)
                return;
            SetConsoleCtrlHandler(ConsoleCtrlHandler, false);
            isSet = false;
        }

        private bool ConsoleCtrlHandler(uint dwCtrlType)
        {
            try
            {
                switch (dwCtrlType)
                {
                    case CTRL_C_EVENT:
                    case CTRL_BREAK_EVENT:
                    case CTRL_CLOSE_EVENT:
                    case CTRL_SHUTDOWN_EVENT:
                        onStop();
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