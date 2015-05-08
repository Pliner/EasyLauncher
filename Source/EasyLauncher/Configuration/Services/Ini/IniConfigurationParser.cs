using System;
using System.IO;
using System.Linq;

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
                    var groupName = complexSectionParts.First();
                    var timeout = complexSectionParts
                        .Skip(1)
                        .Take(1)
                        .Select(int.Parse)
                        .Select(x => TimeSpan.FromSeconds(x))
                        .DefaultIfEmpty(TimeSpan.FromSeconds(1))
                        .First();
                    return new ServicesConfigurationGroup
                    {
                        Name = groupName,
                        Priority = groupPriority--,
                        Timeout = timeout,
                        Services = section.Select(x => new ServiceConfiguration
                        {
                            Name = x.Name,
                            Path = x.Value,
                            Priority = sectionPriority--,
                        }).ToArray()
                    };
                }).ToArray()
            };
        }
    }
}