using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyLauncher
{
    public interface IServiceLauncher
    {
        void Start(IEnumerable<ServiceLaunchParameters> servicesParameters);
        void WaitUntilStop();
    }

    public sealed class ConsoleServicesLauncher : IServiceLauncher
    {
        private readonly object syncLock = new object();
        private readonly TimeSpan serviceStartTimeout = TimeSpan.FromMilliseconds(1000);
        private volatile bool isStarted = false;

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

        public void Start(IEnumerable<ServiceLaunchParameters> servicesParameters)
        {
            lock (syncLock)
            {
                consoleHandler.AddStopHandler(StopAll);
                output.Info("Launching services...\r\n");
                foreach (var serviceParameters in servicesParameters)
                {
                    try
                    {
                        output.Info(string.Format("Starting {0}...", serviceParameters.Name));
                        var process = processLauncher.Launch(serviceParameters);
                        process.OnExit += (sender, args) =>
                        {
                            if (isStarted)
                                return;
                            output.Error(string.Format("Service {0} has stopped", process.Name));
                        };
                        processes.Add(process);
                        output.Info(string.Format("Service {0} launched", serviceParameters.Name));
                    }
                    catch (Exception exception)
                    {
                        output.Error(string.Format("Service {0} failed to launch", serviceParameters.Name), exception);
                    }
                    threadSleeper.Sleep(serviceStartTimeout);
                }
                output.Info("\r\nServices were launched");
                isStarted = true;
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
                threadSleeper.Sleep(serviceStartTimeout);
            }
            consoleHandler.RemoveAll();
            output.Info("\r\nServices were stopped");
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
                    catch
                    {
                    }
                }
                processes.Clear();
            }
        }
    }
}