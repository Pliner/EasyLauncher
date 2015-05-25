using System.IO;
using System.Linq;
using System.Text;
using EasyLauncher.Configuration;
using EasyLauncher.Configuration.Services;
using EasyLauncher.Configuration.Services.Json;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class JsonConfigurationLoaderTest
    {
        private IServicesConfigurationParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new JsonConfigurationParser();
        }

        [Test]
        public void ShouldLoadFullConfiguration()
        {
            using (var stream = 
                @"{
                Groups : [
                    {
                        Name: 'Group',
                        Priority: 2,
                        Services: [
                            {
                                Name: 'Service',
                                Path: 'Path',
                                Priority: 2
                            }
                        ]
                    }
                ]
            }".ToMemoryStream())
            {
                var configuration = parser.Parse(stream);
                Assert.NotNull(configuration);
                Assert.AreEqual(1, configuration.Groups.Count());
                var group = configuration.Groups[0];
                Assert.AreEqual("Group", group.Name);
                Assert.AreEqual(2, group.Priority);
                Assert.AreEqual(1, group.Services.Count());
                var service = group.Services[0];
                Assert.AreEqual("Service", service.Name);
                Assert.AreEqual("Path", service.Path);
                Assert.AreEqual(2, service.Priority);
            }
        }

        [Test]
        public void ShouldLoadEmptyConfiguration()
        {
            using (var stream = "{}".ToMemoryStream())
            {
                var configuration = parser.Parse(stream);
                Assert.NotNull(configuration);
                Assert.IsEmpty(configuration.Groups);
            }
        }
    }
}