using System.Diagnostics;
using System.IO;
using EasyLauncher.Configuration.Services;

namespace EasyLauncher
{
    public interface IProcessLauncher
    {
        IProcess Launch(ServiceConfiguration service);
    }

    public sealed class ProcessLauncher : IProcessLauncher
    {
        private readonly ITemplateSubstitutor templateSubstitutor;

        public ProcessLauncher(ITemplateSubstitutor templateSubstitutor)
        {
            this.templateSubstitutor = templateSubstitutor;
        }

        public IProcess Launch(ServiceConfiguration service)
        {
            var path = templateSubstitutor.Substitute(service.Path);
            var name = templateSubstitutor.Substitute(service.Name);
            var arguments = templateSubstitutor.Substitute(service.Argments);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = path,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = new FileInfo(path).DirectoryName
                },
                EnableRaisingEvents = true
            };
            process.Start();
            return new ProcessAdapter(process) { Name = name };   
        }
    }
}