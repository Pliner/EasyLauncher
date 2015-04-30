using System;
using CommandLine;
using EasyLauncher.Configuration.Services;

namespace EasyLauncher.Configuration.Launch
{
    public interface IServiceLaunchConfigurationParser
    {
        ServiceLaunchConfiguration Parse();
    }

    public sealed class ConsoleServiceLaunchConfigurationParser : IServiceLaunchConfigurationParser
    {
        private readonly string[] args;

        public ConsoleServiceLaunchConfigurationParser(string[] args)
        {
            this.args = args;
        }

        public ServiceLaunchConfiguration Parse()
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
                return new ServiceLaunchConfiguration
                {
                    Filename = options.Filename,
                    ExcludeGroups = options.ExcludeGroups,
                    Type = options.Type,
                    BasePath = options.BasePath
                };
            throw new Exception();
        }

        private class Options
        {
            [OptionArray("excludeGroups", DefaultValue = new string[0])]
            public string[] ExcludeGroups { get; set; }

            [Option("config", Required = true)]
            public string Filename { get; set; }

            [Option("configType", DefaultValue = ServiceConfigurationType.Auto)]
            public ServiceConfigurationType Type { get; set; }

            [Option("basePath", Required = true)]
            public string BasePath { get; set; }
        }
    }
}