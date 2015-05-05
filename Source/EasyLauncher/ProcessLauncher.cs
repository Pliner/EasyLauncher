using System.Diagnostics;
using System.IO;

namespace EasyLauncher
{
    public interface IProcessLauncher
    {
        IProcess Launch(ServiceLaunchParameters serviceLaunchParameters);
    }

    public sealed class ProcessLauncher : IProcessLauncher
    {
        public IProcess Launch(ServiceLaunchParameters serviceLaunchParameters)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = serviceLaunchParameters.Path,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = new FileInfo(serviceLaunchParameters.Path).DirectoryName
                },
                EnableRaisingEvents = true
            };
            process.Start();
            return new ProcessAdapter(process) { Name = serviceLaunchParameters.Name };   
        }
    }
}