using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EasyLauncher.Configuration.Services;

namespace EasyLauncher
{
    public interface IServiceLauncher
    {
        void Start(ServicesConfiguration configuration);
        void WaitUntilStop();
    }

    public sealed class ConsoleServicesLauncher : IServiceLauncher
    {
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

        public void Start(ServicesConfiguration configuration)
        {
            if (status.TryChangeState(ServiceLauncherState.Stopped, ServiceLauncherState.Starting))
            {
                consoleHandler.AddStopHandler(StopAll);
                output.Info("Launching services...\r\n");
                foreach (var servicesGroup in configuration.Groups.OrderByDescending(x => x.Priority))
                {
                    foreach (var service in servicesGroup.Services.OrderByDescending(x => x.Priority))
                    {
                        if (!status.HasState(ServiceLauncherState.Starting))
                            return;
                        try
                        {
                            output.Info(string.Format("Starting {0}...", service.Name));
                            var process = processLauncher.Launch(service);
                            process.OnExit += (sender, args) =>
                            {
                                processes.Remove(process);
                                if (status.HasState(ServiceLauncherState.Stopping) || status.HasState(ServiceLauncherState.Stopped))
                                    return;
                                output.Error(string.Format("Service {0} has exited", process.Name));
                            };
                            processes.Add(process);
                            output.Info(string.Format("Service {0} launched", servicesGroup.Name));
                        }
                        catch (Exception exception)
                        {
                            output.Error(string.Format("Service {0} failed to launch", servicesGroup.Name), exception);
                        }
                    }
                    threadSleeper.Sleep(servicesGroup.Timeout);
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
                    processes.Remove(process.Key);
                }
                status.ChangeState(ServiceLauncherState.Stopped);
            }
        }
    }
}