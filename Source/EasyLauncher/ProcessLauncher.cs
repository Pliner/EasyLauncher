using System.Diagnostics;
using System.IO;

namespace EasyLauncher
{
    public interface IProcessLauncher
    {
        IProcess Launch(ServiceParameters serviceParameters);
    }

    public sealed class ProcessLauncher : IProcessLauncher
    {
        public IProcess Launch(ServiceParameters serviceParameters)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = serviceParameters.Path,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = new FileInfo(serviceParameters.Path).DirectoryName
                },
                EnableRaisingEvents = true
            };
            process.Start();
            return new ProcessAdapter(process) { Name = serviceParameters.Name };   
        }
    }
}