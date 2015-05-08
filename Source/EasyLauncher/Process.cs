using System;
using System.Diagnostics;
using System.Management;

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
            KillProcessTree(process.Id);
        }

        private static void KillProcessTree(int pid)
        {
            var processSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            foreach (var proccess in processSearcher.Get())
            {
                var managementObject = (ManagementObject) proccess;
                KillProcessTree(Convert.ToInt32(managementObject["ProcessID"]));
            }
            try
            {
                var process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch
            {
            }
        }

        public event EventHandler OnExit
        {
            add { process.Exited += value; }
            remove { process.Exited -= value; }
        }
    }
}