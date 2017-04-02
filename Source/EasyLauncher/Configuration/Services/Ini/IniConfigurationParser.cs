using System;
using System.IO;
using System.Linq;
using SharpConfig;

namespace EasyLauncher.Configuration.Services.Ini
{
    public sealed class IniConfigurationParser : IServicesConfigurationParser
    {
        public ServicesConfiguration Parse(Stream stream)
        {
            var configuration = SharpConfig.Configuration.LoadFromStream(stream);
            var groupPriority = 0;
            var sectionPriority = 0;

            return new ServicesConfiguration
            {
                Groups = configuration.Select(section =>
                {
                    var complexSectionName = section.Name ?? "Default";
                    var complexSectionParts = complexSectionName.Split('|');
                    var groupName = complexSectionParts.ElementAt(0);
                    var timeoutSeconds = int.Parse(complexSectionParts.ElementAtOrDefault(1) ?? "1");
                    return new ServicesConfigurationGroup
                    {
                        Name = groupName,
                        Priority = groupPriority--,
                        Timeout = TimeSpan.FromSeconds(timeoutSeconds),
                        Services = section.Select(setting => ParseServiceSetting(setting, sectionPriority)).ToArray()
                    };
                }).ToArray()
            };
        }

        private static ServiceConfiguration ParseServiceSetting(Setting setting, int sectionPriority)
        {
            var argumentsIndex = setting.Value.IndexOf(' ');

            var servicePath = argumentsIndex > 0
                ? setting.Value.Substring(0, argumentsIndex)
                : setting.Value;

            var serviceArguments = argumentsIndex > 0
                ? setting.Value.Substring(argumentsIndex + 1)
                : null;

            return new ServiceConfiguration
            {
                Name = setting.Name,
                Path = servicePath,
                Argments = serviceArguments,
                Priority = sectionPriority--,
            };
        }
    }
}