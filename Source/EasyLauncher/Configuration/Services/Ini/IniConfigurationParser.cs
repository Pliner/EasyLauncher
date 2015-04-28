using System.IO;
using System.Linq;

namespace EasyLauncher.Configuration.Services.Ini
{
    public sealed class IniConfigurationParser : IServicesConfigurationParser
    {
        private const string ServiceKeyPrefix = "Service.";

        public ServicesConfiguration Parse(Stream stream)
        {
            var configuration = SharpConfig.Configuration.LoadFromStream(stream);
            var groupPriority = 0;
            var sectionPriority = 0;

            return new ServicesConfiguration
            {
                Groups = configuration.Select(section => new ServicesConfigurationGroup
                {
                    Name = section.Name ?? Defaults.DefaultGroupName,
                    Priority = groupPriority--,
                    Services = section.Where(x => x.Name.StartsWith(ServiceKeyPrefix))
                        .Select(x => new ServiceConfiguration
                        {
                            Name = x.Name.Replace(ServiceKeyPrefix, ""),
                            Path = x.Value,
                            Priority = sectionPriority--,
                        }).ToArray()
                }).ToArray()
            };
        }
    }
}