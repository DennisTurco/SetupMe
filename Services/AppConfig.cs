using setupme.Interfaces;

namespace setupme.Services
{
    public class AppConfig : IAppConfig
    {
        public string ConfigFilePath { get; set; } = string.Empty;
    }
}
