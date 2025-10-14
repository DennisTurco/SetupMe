using Cocona;
using setupme.Entities;
using setupme.Exceptions;
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
                throw new YamlFormatException($"Yaml configuration {name} does not exist");
            }

            RunConfiguration(config);
        }

        private Dictionary<string, Config> DeserializeYamlConfig()
        {
            var deserializer = new DeserializerBuilder().Build();
            var filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _appConfig.ConfigFilePath));
            var yamlText = File.ReadAllText(filePath);

            try
            {
                return deserializer.Deserialize<Dictionary<string, Config>>(yamlText);
            }
            catch (Exception)
            {
                throw new YamlFormatException("Yaml configuration il bad formatted, you can edit it by run 'setupme edit' command");
            }
        }

        private void RunConfiguration(Config config)
        {
            Console.WriteLine($"Running configuration {config.Name}");
        }

        private void SimulateConfiguration(Config config)
        {

        }
    }
}