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
        void Start(ServiceParameters serviceParameters);
        void StopAll();
        bool IsAllStopped();
    }

    public sealed class ServiceLauncher : IServiceLauncher
    {
        private readonly object syncLock = new object();

        private readonly List<IProcess> processes = new List<IProcess>(); 
        private readonly IProcessLauncher processLauncher;
        private readonly IOutput output;

        public ServiceLauncher(IProcessLauncher processLauncher, IOutput output)
        {
            this.processLauncher = processLauncher;
            this.output = output;
        }

        public void Start(ServiceParameters serviceParameters)
        {
            lock (syncLock)
            {
                try
                {
                    output.Info(string.Format("Starting {0}...", serviceParameters.Name));
                    var process = processLauncher.Launch(serviceParameters);
                    process.OnExit += (sender, args) => output.Error(string.Format("ServiceConfiguration {0} has exited", process.Name));
                    processes.Add(process);
                    output.Info(string.Format("Service {0} started", serviceParameters.Name));
                }
                catch (Exception exception)
                {
                    output.Error(string.Format("Service {0} failed to start", serviceParameters.Name), exception);
                }
            }
        }

        public void StopAll()
        {
            lock (syncLock)
            {
                foreach (var process in processes)
                    try
                    {
                        process.Kill();
                    }
                    catch (Exception exception)
                    {
                        output.Error(string.Format("Service {0} failed to stop", process.Name), exception);
                    }
                processes.Clear();
            }
        }

        public bool IsAllStopped()
        {
            lock (syncLock)
                return processes.All(x => x.IsStopped);
        }
    }
}