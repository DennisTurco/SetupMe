using CliWrap.Builders;
using setupme.Entities;
using setupme.Exceptions;
using setupme.Services;
using SetupMe.Interfaces;

namespace SetupMe.Installers
{
    public class WingetInstaller : IPackageInstaller
    {
        private readonly string _wingetPath;

        public WingetInstaller()
        {
            _wingetPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "winget.exe");
        }

        public async Task InstallPackage(string packageName, Flags flags)
        {
            if (!IsWingetInstalled())
            {
                await InstallWinget();
            }

            Console.WriteLine($"⬇ Installing package {packageName} via winget");

            Action<ArgumentsBuilder> arguments = args =>
            {
                args.Add("install").Add(packageName);

                if (!string.IsNullOrEmpty(flags.Version))
                    args.Add("--version").Add(flags.Version);
                if (flags.Force)
                    args.Add("--force");
                if (flags.Quiet)
                    args.Add("--silent");
            };

            var exitCode = await CliWrapperService.ExecuteCliCommand("winget", arguments);
            if (exitCode != 0)
            {
                throw new Exception($"Winget failed to install {packageName}. Exit code: {exitCode}");
            }
        }

        private bool IsWingetInstalled()
        {
            return File.Exists(_wingetPath);
        }

        private async Task InstallWinget()
        {
            try
            {
                Console.WriteLine("Winget is not installed. Installing App Installer from Microsoft Store...");

                Action<ArgumentsBuilder> arguments = args => {
                    args.Add("-NoProfile")
                        .Add("-ExecutionPolicy").Add("Bypass")
                        .Add("-Command")
                        .Add("Add-AppxPackage -Path 'https://aka.ms/Microsoft.DesktopAppInstaller_8wekyb3d8bbwe.appxbundle'");
                };

                var exitCode = await CliWrapperService.ExecuteCliCommand("powershell.exe", arguments);

                if (exitCode == 0 && File.Exists(_wingetPath))
                {
                    Console.WriteLine("Winget installed successfully.");
                }
                else
                {
                    throw new PackageInstallerException("Failed to install Winget. Make sure your system supports App Installer.");
                }
            }
            catch (Exception ex)
            {
                throw new PackageInstallerException($"Error installing Winget: {ex.Message}");
            }
        }
    }
}