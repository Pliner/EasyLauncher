using System.IO;
using System.Linq;
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
            var threadSleeper = new ThreadSleeper();
            var consoleHandler = new ConsoleHandler();
            var launcher = new ConsoleServicesLauncher(processLauncher, consoleOutput, consoleHandler, threadSleeper);
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
            var servicesParameters = serviceConfigurations.Select(x => new ServiceParameters
            {
                Name = x.Name,
                Path = x.Path.Replace("$BasePath$", launchConfiguration.BasePath)
            });
            launcher.Start(servicesParameters);
            launcher.WaitUntilStop();
        }

       
    }
}