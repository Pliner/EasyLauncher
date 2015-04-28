using System.IO;

namespace EasyLauncher.Configuration.Services
{
    public interface IServicesConfigurationParser
    {
        ServicesConfiguration Parse(Stream stream);
    }
}