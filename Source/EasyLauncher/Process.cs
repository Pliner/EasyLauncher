using System;
using System.Diagnostics;

namespace EasyLauncher
{
    public interface IProcess
    {
        string Name { get; }
        bool IsStopped { get; }
        void Kill();
        event EventHandler OnExit;
    }

    public sealed class ProcessAdapter : IProcess
    {
        private readonly Process process;

        public ProcessAdapter(Process process)
        {
            this.process = process;
        }

        public string Name { get; set; }

        public bool IsStopped
        {
            get { return process.HasExited; }
        }

        public void Kill()
        {
            process.Kill();
        }

        public event EventHandler OnExit
        {
            add { process.Exited += value; }
            remove { process.Exited -= value; }
        }
    }
}