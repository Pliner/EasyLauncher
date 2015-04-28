namespace EasyLauncher.Configuration.Services
{
    public class ServicesConfigurationGroup
    {
        private ServiceConfiguration[] services;

        public string Name { get; set; }

        public int Priority { get; set; }

        public ServiceConfiguration[] Services
        {
            get { return services ?? (services = new ServiceConfiguration[0]); }
            set { services = value; }
        }
    }
}