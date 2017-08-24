namespace InternalsVisibleToFixer.Configuration
{
    public static class ConfigurationManager
    {
        public static IConfiguration Instance { get; } = new DefaultConfiguration(); 
    }
}