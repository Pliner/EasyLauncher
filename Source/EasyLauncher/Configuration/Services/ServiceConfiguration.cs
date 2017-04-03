namespace EasyLauncher.Configuration.Services
{
    public sealed class ServiceConfiguration
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Argments { get; set; }
        public int Priority { get; set; }
    }
}