using System;
using System.Diagnostics;
using System.Management;

namespace EasyLauncher
{
    public interface IProcess
    {
        string Name { get; }
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

        public void Kill()
        {
            KillProcessTree(process.Id);
        }

        private static void KillProcessTree(int pid)
        {
            try
            {
                var processSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
                foreach (var proccess in processSearcher.Get())
                {
                    var managementObject = (ManagementObject)proccess;
                    KillProcessTree(Convert.ToInt32(managementObject["ProcessID"]));
                }
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