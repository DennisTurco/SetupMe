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
            config.Name = name;

            if (config == null)
            {
                throw new YamlFormatException($"Yaml configuration {name} does not exist");
            }

            Console.WriteLine($"Running configuration {name}");

            try
            {
                var uninstallCommand = new UninstallCommand(_installers);
                foreach (var options in config.UninstallOptions)
                {
                    await UninstallFromConfiguration(name, options, uninstallCommand);
                }

                var upgradeCommand = new UpgradeCommand(_installers);
                foreach (var options in config.UpgradeOptions)
                {
                    await UpgradeFromConfiguration(name, options, upgradeCommand);
                }

                var installCommand = new InstallCommand(_installers);
                foreach (var options in config.InstallOptions)
                {
                    await InstallFromConfiguration(name, options, installCommand);
                }

                foreach (var action in config.Actions)
                {
                    await RunAction(action);
                }
            } catch (Exception ex)
            {
                Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
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

        private async Task InstallFromConfiguration(string configName, PackageOptions options, InstallCommand installCommand)
        {
            Console.WriteLine($"Installing packages from configuration {configName}");

            var flags = options.Flags;
            foreach (var package in options.Packages)
            {
                await installCommand.InstallAsync(package, null, flags.Version, flags.Force, flags.Source, flags.Quiet, flags.Confirm);
            }
        }

        private async Task UninstallFromConfiguration(string configName, PackageOptions options, UninstallCommand uninstallCommand)
        {
            Console.WriteLine($"Uninstalling packages from configuration {configName}");

            var flags = options.Flags;
            foreach (var package in options.Packages)
            {
                await uninstallCommand.UninstallAsync(package, null, flags.Version, flags.AllVersions, flags.Force, flags.Source, flags.Quiet, flags.Confirm);
            }
        }

        private async Task UpgradeFromConfiguration(string configName, PackageOptions options, UpgradeCommand upgradeCommand)
        {
            Console.WriteLine($"Upgrading packages from configuration {configName}");

            var flags = options.Flags;
            foreach (var package in options.Packages)
            {
                await upgradeCommand.UpgradeAsync(package, null, flags.Version, flags.Force, flags.Source, flags.Quiet, flags.Confirm);
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
    }
}