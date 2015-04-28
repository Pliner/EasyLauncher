using System.IO;
using System.Linq;
using System.Threading;
using EasyLauncher.Configuration.Launch;
using EasyLauncher.Configuration.Services;
using EasyLauncher.Configuration.Services.Ini;
using EasyLauncher.Configuration.Services.Json;

namespace EasyLauncher
{
    public static class EntryPoint
    {
        public static void Main(string[] args)
        {

            var launchConfigurationParser = new ConsoleServiceLaunchConfigurationParser(args);
            var launchConfiguration = launchConfigurationParser.Parse();
            var processLauncher = new ProcessLauncher();
            var consoleOutput = new ConsoleOutput();
            var launcher = new ServiceLauncher(processLauncher, consoleOutput);
            
            var configurationParser = (launchConfiguration.Type == ServiceConfigurationType.Ini || launchConfiguration.Type == ServiceConfigurationType.Auto)
                ? (IServicesConfigurationParser)new IniConfigurationParser()
                : new JsonConfigurationParser();
            ServicesConfiguration configuration;
            using (var file = File.OpenRead(launchConfiguration.Filename))
                configuration = configurationParser.Parse(file);
            var serviceConfigurations = configuration.Groups
                .OrderByDescending(x => x.Priority)
                .SelectMany(x => x.Services)
                .OrderByDescending(x => x.Priority);
            foreach (var serviceConfiguration in serviceConfigurations)
            {
                launcher.Start(new ServiceParameters
                {
                    Name = serviceConfiguration.Name,
                    Path = serviceConfiguration.Path.Replace("$BasePath$", launchConfiguration.BasePath)
                });
            }


            var consoleHandler = new ConsoleHandler(() => launcher.StopAll());
            consoleHandler.Set();

            while (!launcher.IsAllStopped())
                Thread.Sleep(500);

            consoleHandler.Unset();
        }

       
    }
}