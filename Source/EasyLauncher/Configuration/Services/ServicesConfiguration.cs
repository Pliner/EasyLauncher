namespace EasyLauncher.Configuration.Services
{
    public sealed class ServicesConfiguration 
    {
        private ServicesConfigurationGroup[] groups;

        public ServicesConfigurationGroup[] Groups
        {
            get { return groups ?? (groups = new ServicesConfigurationGroup[0]); }
            set { groups = value; }
        }
    }
}