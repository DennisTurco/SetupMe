using CliWrap.Builders;
using Cocona;
using setupme.Entities;
using setupme.Exceptions;
using setupme.Interfaces;
using setupme.Services;
using SetupMe.Interfaces;
using YamlDotNet.Serialization;

namespace SetupMe.Commands
{
    public class RunConfigurationCommand
    {
        private readonly IAppConfig _appConfig;
        private readonly IEnumerable<IPackageInstaller> _installers;

        public RunConfigurationCommand(IAppConfig appConfig, IEnumerable<IPackageInstaller> installers) 
        {
            _appConfig = appConfig;
            _installers = installers;
        }

        [Command("run", Description = "Run a configured yaml file")]
        public async Task RunConfigurationAsync(
            [Argument(Description = "Configuration name to run")] string name
        )
        {
            var configs = DeserializeYamlConfig();
            var config = configs.FirstOrDefault(kv => kv.Key == name).Value;

            if (config == null)
            {
                throw new YamlFormatException($"Yaml configuration {name} does not exist");
            }

            var installCommand = new InstallCommand(_installers);
            foreach (var installOption in config.InstallOptions)
            {
                await RunConfiguration(name, installOption, installCommand);
            }

            foreach (var action in config.Actions)
            {
                await RunAction(action);
            }
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

        private async Task RunConfiguration(string configName, InstallOptions installOption, InstallCommand installCommand)
        {
            Console.WriteLine($"Running configuration {configName}");

            var flags = installOption.Flags;
            foreach (var package in installOption.Packages)
            {
                await installCommand.InstallAsync(package, null, flags.Version, flags.Force, flags.Source, flags.Quiet, flags.Confirm);
            }
        }

        private async Task RunAction(CustomAction action)
        {
            Console.WriteLine($"Running action {action.Name}");

            if (string.IsNullOrEmpty(action.Run))
            {
                throw new RunConfigurationException($"Action {action.Name} has not a run command setted");
            }

            var parts = action.Run.Split(' ', 2);
            var processName = parts[0];
            var argsText = parts.Length > 1 ? parts[1] : string.Empty;

            Action<ArgumentsBuilder> arguments = args =>
            {
                if (!string.IsNullOrWhiteSpace(argsText))
                    args.Add(argsText);
            };

            var exitCode = await CliWrapperService.ExecuteCliCommand(processName, arguments);

            if (exitCode != 0)
            {
                throw new RunConfigurationException($"Action {action.Name} exited with error");
            }
        }

        private void SimulateConfiguration(Config config)
        {

        }
    }
}