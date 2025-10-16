using CliWrap.Builders;
using setupme.Entities;
using setupme.Exceptions;
using setupme.Services;
using SetupMe.Interfaces;

namespace SetupMe.Installers
{
    public class ChocolatelyInstaller : IPackageInstaller
    {
        private readonly string _chocoPath;

        public ChocolatelyInstaller()
        {
            _chocoPath = Environment.ExpandEnvironmentVariables(@"%ProgramData%\chocolatey\bin\choco.exe");
        }
        
        public async Task InstallPackage(string packageName, Flags flags)
        {
            await InstallChocolatelyIfMissing();

            Console.WriteLine($"Installing package {packageName} via chocolately");

            Action<ArgumentsBuilder> arguments = args =>
            {
                args.Add("install").Add(packageName);

                if (!string.IsNullOrEmpty(flags.Version))
                    args.Add("--version").Add(flags.Version);
                if (flags.Force)
                    args.Add("--force");
                if (flags.Quiet)
                    args.Add("--silent");
                if (flags.Confirm)
                    args.Add("--yes");
            };
            
            await ExecuteCommand(packageName, arguments, "install");
        }

        public async Task UninstallPackage(string packageName, Flags flags)
        {
            await InstallChocolatelyIfMissing();

            Console.WriteLine($"Uninstalling package {packageName} via chocolately");

            Action<ArgumentsBuilder> arguments = args =>
            {
                args.Add("uninstall").Add(packageName);

                if (!string.IsNullOrEmpty(flags.Version))
                    args.Add("--version").Add(flags.Version);
                if (flags.AllVersions)
                    args.Add("--allversions");
                else if (flags.Force)
                    args.Add("--force");
                if (flags.Quiet)
                    args.Add("--silent").Add("--limitoutput");
                if (flags.Confirm)
                    args.Add("--yes");
            };

            await ExecuteCommand(packageName, arguments, "uninstall");
        }

        public async Task UpgradePackage(string packageName, Flags flags)
        {
            await InstallChocolatelyIfMissing();

            Console.WriteLine($"Upgrading package {packageName} via chocolately");

            Action<ArgumentsBuilder> arguments = args =>
            {
                args.Add("upgrade").Add(packageName);

                if (!string.IsNullOrEmpty(flags.Version))
                    args.Add("--version").Add(flags.Version);
                else if (flags.Force)
                    args.Add("--force");
                if (flags.Quiet)
                    args.Add("--silent");
                if (flags.Confirm)
                    args.Add("--yes");
            };

            await ExecuteCommand(packageName, arguments, "upgrade");
        }

        public async Task SearchPackage(string packageName)
        {
            await InstallChocolatelyIfMissing();

            Console.WriteLine($"Serach package {packageName} via chocolately");

            Action<ArgumentsBuilder> arguments = args =>
            {
                args.Add("serach").Add(packageName);
            };

            await ExecuteCommand(packageName, arguments, "search");
        }

        private async Task ExecuteCommand(string packageName, Action<ArgumentsBuilder> arguments, string actionName)
        {
            var exitCode = await CliWrapperService.ExecuteCliCommand("choco", arguments);
            if (exitCode != 0)
            {
                throw new Exception($"Chocolately failed to {actionName} {packageName}. Exit code: {exitCode}");
            }
        }

        private async Task InstallChocolatelyIfMissing()
        {
            if (!File.Exists(_chocoPath))
            {
                await InstallChocolately();
            }
        }

        private async Task InstallChocolately()
        {
            try
            {
                Action<ArgumentsBuilder> arguments = args =>
                {
                    args.Add("-NoProfile")
                        .Add("-ExecutionPolicy").Add("Bypass")
                        .Add("-Command")
                        .Add(@"Set-ExecutionPolicy Bypass -Scope Process -Force; " +
                                @"[System.Net.ServicePointManager]::SecurityProtocol = " +
                                @"[System.Net.ServicePointManager]::SecurityProtocol -bor 3072; " +
                                @"iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))");
                };

                var exitCode = await CliWrapperService.ExecuteCliCommand("powershell.exe", arguments);

                if (exitCode == 0 && File.Exists(_chocoPath))
                {
                    Console.WriteLine("Chocolatey installed successfully.");
                }
                else
                {
                    throw new PackageInstallerException($"Failed to install Chocolatey. Exit code: {exitCode}");
                }
            }
            catch (Exception ex)
            {
                throw new PackageInstallerException($"Error installing Chocolatey: {ex.Message}");
            }
        }
    }
}