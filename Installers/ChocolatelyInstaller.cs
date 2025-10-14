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
            if (!IsChocolateyInstalled())
            {
                await InstallChocolately();
            }

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
            
            var exitCode = await CliWrapperService.ExecuteCliCommand("choco", arguments);

            if (exitCode != 0)
            {
                throw new Exception($"Winget failed to install {packageName}. Exit code: {exitCode}");
            }
        }

        private bool IsChocolateyInstalled()
        {
            return File.Exists(_chocoPath);
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