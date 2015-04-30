using EasyLauncher.Configuration.Services;

namespace EasyLauncher.Configuration.Launch
{
    public class ServiceLaunchConfiguration
    {
        public string[] ExcludeGroups { get; set; }
        public string Filename { get; set; }
        public ServiceConfigurationType Type { get; set; }
        public string BasePath { get; set; }
    }
}