using System;

namespace EasyLauncher
{
    public interface IOutput
    {
        void Info(string message);
        void Error(string message, Exception exception);
        void Error(string message);
    }

    public sealed class ConsoleOutput : IOutput
    {
        private readonly object outputLock = new object();

        public void Info(string message)
        {
            Output(message, ConsoleColor.Green);
        }

        public void Error(string message, Exception exception)
        {
            Output(string.Format("{0} ---> {1}", message, exception), ConsoleColor.Red);
        }

        public void Error(string message)
        {
            Output(message, ConsoleColor.Red);
        }

        private void Output(string message, ConsoleColor color)
        {
            lock (outputLock)
            {
                var oldColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ForegroundColor = oldColor;
            }
        }
    }
}