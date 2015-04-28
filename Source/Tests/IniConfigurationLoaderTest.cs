using System.Linq;
using EasyLauncher.Configuration;
using EasyLauncher.Configuration.Services.Ini;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class IniConfigurationLoaderTest
    {
        private IniConfigurationParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new IniConfigurationParser();
        }

        [Test]
        public void ShouldLoadEmptyConfiguration()
        {
            using (var stream = "\r\n".ToMemoryStream())
            {
                var configuration = parser.Parse(stream);
                Assert.NotNull(configuration);
                Assert.IsEmpty(configuration.Groups);
            }
        }

        [Test]
        public void ShouldLoadFullConfiguration()
        {
            using (var stream = @"
                [GroupName]
                Service.ServiceName = ServicePath
                ".ToMemoryStream())
            {
                var configuration = parser.Parse(stream);
                Assert.NotNull(configuration);
                Assert.AreEqual(1, configuration.Groups.Count());
                var group = configuration.Groups[0];
                Assert.AreEqual("GroupName", group.Name);
                Assert.AreEqual(0, group.Priority);
                Assert.AreEqual(1, group.Services.Count());
                var service = group.Services[0];
                Assert.AreEqual("ServiceName", service.Name);
                Assert.AreEqual("ServicePath", service.Path);
                Assert.AreEqual(0, service.Priority);
            }
        }
    }
}