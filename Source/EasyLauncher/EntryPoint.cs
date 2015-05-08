using System.Collections.Generic;
using System.IO;
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
            var consoleOutput = new ConsoleOutput();
            var threadSleeper = new ThreadSleeper();
            var consoleHandler = new ConsoleHandler();
            var serviceLauncherStatus = new ServiceLauncherStatus();
            var configurationParser = (launchConfiguration.Type == ServiceConfigurationType.Ini ||
                                       launchConfiguration.Type == ServiceConfigurationType.Auto)
                ? (IServicesConfigurationParser) new IniConfigurationParser()
                : new JsonConfigurationParser();
            var templates = new Dictionary<string, string>
            {
                {
                    "$BasePath$", launchConfiguration.BasePath
                }
            };
            var templateSubstitutor = new TemplateSubstitutor(templates);
            var processLauncher = new ProcessLauncher(templateSubstitutor);
            var launcher = new ConsoleServicesLauncher(processLauncher, consoleOutput, consoleHandler, serviceLauncherStatus, threadSleeper);
            ServicesConfiguration configuration;
            using (var file = File.OpenRead(launchConfiguration.Filename))
                configuration = configurationParser.Parse(file);

            launcher.Start(configuration);
            launcher.WaitUntilStop();
        }
    }
}