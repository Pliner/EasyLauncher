using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<IProcess, object> processes = new ConcurrentDictionary<IProcess, object>();
        private readonly IProcessLauncher processLauncher;
        private readonly IOutput output;
        private readonly IConsoleHandler consoleHandler;
        private readonly IServiceLauncherStatus status;
        private readonly IThreadSleeper threadSleeper;

        public ConsoleServicesLauncher(IProcessLauncher processLauncher,
            IOutput output,
            IConsoleHandler consoleHandler,
            IServiceLauncherStatus status,
            IThreadSleeper threadSleeper)
        {
            this.processLauncher = processLauncher;
            this.output = output;
            this.consoleHandler = consoleHandler;
            this.status = status;
            this.threadSleeper = threadSleeper;
        }

        public void Start(IEnumerable<ServiceLaunchParameters> servicesParameters)
        {
            if (status.TryChangeState(ServiceLauncherState.Stopped, ServiceLauncherState.Starting))
            {
                consoleHandler.AddStopHandler(StopAll);
                output.Info("Launching services...\r\n");
                foreach (var serviceParameters in servicesParameters)
                {
                    if (!status.HasState(ServiceLauncherState.Starting))
                        return;
                    try
                    {
                        output.Info(string.Format("Starting {0}...", serviceParameters.Name));
                        var process = processLauncher.Launch(serviceParameters);
                        process.OnExit += (sender, args) =>
                        {
                            object value;
                            processes.TryRemove(process, out value);
                            if (status.HasState(ServiceLauncherState.Stopping))
                                return;
                            output.Error(string.Format("Service {0} has exited", process.Name));
                        };
                        processes.TryAdd(process, null);
                        output.Info(string.Format("Service {0} launched", serviceParameters.Name));
                    }
                    catch (Exception exception)
                    {
                        output.Error(string.Format("Service {0} failed to launch", serviceParameters.Name), exception);
                    }
                    threadSleeper.Sleep(serviceStartTimeout);
                }
                status.ChangeState(ServiceLauncherState.Started);
                output.Info("\r\nServices were launched");
            }
        }

        public void WaitUntilStop()
        {
            while (true)
            {
                if (status.HasState(ServiceLauncherState.Stopped))
                    break;
                threadSleeper.Sleep(serviceStartTimeout);
            }
            consoleHandler.RemoveAll();
            output.Info("\r\nServices were stopped");
        }

        private void StopAll()
        {
            if (status.TryChangeState(ServiceLauncherState.Started, ServiceLauncherState.Stopping) || status.TryChangeState(ServiceLauncherState.Starting, ServiceLauncherState.Stopping))
            {
                foreach (var process in processes)
                {
                    try
                    {
                        process.Key.Kill();
                    }
                    catch
                    {
                    }
                    object value;
                    processes.TryRemove(process.Key, out value);
                }
                status.ChangeState(ServiceLauncherState.Stopped);
            }
        }
    }
}