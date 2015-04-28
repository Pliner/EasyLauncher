using System.IO;
using Newtonsoft.Json;

namespace EasyLauncher.Configuration.Services.Json
{
    public sealed class JsonConfigurationParser : IServicesConfigurationParser
    {
        public ServicesConfiguration Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ServicesConfiguration>(json);
            }
        }
    }
}