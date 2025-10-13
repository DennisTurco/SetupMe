using Cocona;
using setupme.Entities;
using setupme.Interfaces;
using YamlDotNet.Serialization;

namespace SetupMe.Commands
{
    public class RunConfigurationCommand
    {
        private readonly IAppConfig _appConfig;

        public RunConfigurationCommand(IAppConfig appConfig) 
        {
            _appConfig = appConfig;
        }

        [Command("run", Description = "Run a configured yaml file")]
        public void RunConfiguration(
            [Argument(Description = "Configuration name to run")] string name
        )
        {
            var configs = DeserializeYamlConfig();
            var config = configs.FirstOrDefault(kv => kv.Key == name).Value;

            if (config == null)
            {
                Console.Error.WriteLine($"Error: configuration {name} does not exist");
                return;
            }

            Console.WriteLine($"Running configuration {name}");
        }

        private Dictionary<string, Config> DeserializeYamlConfig()
        {
            var deserializer = new DeserializerBuilder().Build();
            var filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _appConfig.ConfigFilePath));
            var yamlText = File.ReadAllText(filePath);
            return deserializer.Deserialize<Dictionary<string, Config>>(yamlText);
        }
    }
}