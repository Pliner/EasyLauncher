using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyLauncher
{
    public sealed class ServiceParameters
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public interface IServiceLauncher
    {
        void Start(IEnumerable<ServiceParameters> servicesParameters);
        void WaitUntilStop();
    }

    public sealed class ConsoleServicesLauncher : IServiceLauncher
    {
        private readonly object syncLock = new object();
        private readonly TimeSpan timeout = TimeSpan.FromMilliseconds(500);

        private readonly List<IProcess> processes = new List<IProcess>(); 
        private readonly IProcessLauncher processLauncher;
        private readonly IOutput output;
        private readonly IConsoleHandler consoleHandler;
        private readonly IThreadSleeper threadSleeper;

        public ConsoleServicesLauncher(IProcessLauncher processLauncher,
                               IOutput output, 
                               IConsoleHandler consoleHandler,
                               IThreadSleeper threadSleeper)
        {
            this.processLauncher = processLauncher;
            this.output = output;
            this.consoleHandler = consoleHandler;
            this.threadSleeper = threadSleeper;
        }

        public void Start(IEnumerable<ServiceParameters> servicesParameters)
        {
            consoleHandler.AddStopHandler(StopAll);
            foreach (var serviceParameters in servicesParameters)
            {
                lock (syncLock)
                {
                    try
                    {
                        output.Info(string.Format("Starting {0}...", serviceParameters.Name));
                        var process = processLauncher.Launch(serviceParameters);
                        process.OnExit += (sender, args) => output.Error(string.Format("ServiceConfiguration {0} has stopped", process.Name));
                        processes.Add(process);
                        output.Info(string.Format("Service {0} started", serviceParameters.Name));
                    }
                    catch (Exception exception)
                    {
                        output.Error(string.Format("Service {0} failed to start", serviceParameters.Name), exception);
                    }
                }
                threadSleeper.Sleep(timeout);
            }
        }

        public void WaitUntilStop()
        {
            while (true)
            {
                lock (syncLock)
                {
                    if (processes.All(x => x.IsStopped))
                        break;
                }
                threadSleeper.Sleep(timeout);
            }
            consoleHandler.RemoveAll();
        }

        private void StopAll()
        {
            lock (syncLock)
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception exception)
                    {
                        output.Error(string.Format("Service {0} failed to stop", process.Name), exception);
                    }
                }
                processes.Clear();
            }
        }
    }
}