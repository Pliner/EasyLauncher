using System;
using CommandLine;

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
                    Groups = options.Groups,
                    Type = options.Type,
                    BasePath = options.BasePath
                };
            throw new Exception();
        }

        private class Options
        {
            [OptionArray('g', "groups", DefaultValue = new [] {"Default"})]
            public string[] Groups { get; set; }

            [Option('c', "config", Required = true)]
            public string Filename { get; set; }

            [Option('t', "configType", DefaultValue = ServiceConfigurationType.Auto)]
            public ServiceConfigurationType Type { get; set; }

            [Option('b', "basePath", Required = true)]
            public string BasePath { get; set; }
        }
    }
}