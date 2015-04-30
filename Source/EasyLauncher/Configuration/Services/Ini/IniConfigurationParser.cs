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
                Groups = configuration.Select(section => new ServicesConfigurationGroup
                {
                    Name = section.Name ?? "Default",
                    Priority = groupPriority--,
                    Services = section.Select(x => new ServiceConfiguration
                        {
                            Name = x.Name,
                            Path = x.Value,
                            Priority = sectionPriority--,
                        }).ToArray()
                }).ToArray()
            };
        }
    }
}