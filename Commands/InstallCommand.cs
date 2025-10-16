using Cocona;
using SetupMe.Interfaces;
using setupme.Entities;
using setupme.Exceptions;
using setupme.Commands;

namespace SetupMe.Commands
{
    public class InstallCommand : PackageCommandBase
    {
        public InstallCommand(IEnumerable<IPackageInstaller> installers) : base(installers) { }

        [Command("install", Description = "Installs a package from the specified source or from all available sources.")]
        public async Task InstallAsync(
            [Argument(Description = "The name of the package to install.")] string package,
            [Option('v', Description = "Specify the package version to install. If omitted, the latest version is used.")] string? version = null,
            [Option('f', Description = "Force reinstallation, even if the package is already installed.")] bool force = false,
            [Option('s', Description = "Specify the package source (e.g. 'winget' or 'choco'). If omitted, all sources are tried.")] string? source = null,
            [Option('q', Description = "Run in quiet mode with minimal console output.")] bool quiet = false,
            [Option('y', Description = "Automatically confirm all prompts and proceed without asking.")] bool yes = false
        )
        {
            var installer = GetInstallerBySource(source);

            var flags = new Flags
            {
                Version = version ?? string.Empty,
                Force = force,
                Source = source ?? string.Empty,
                Quiet = quiet,
                Confirm = yes
            };

            try
            {
                if (installer != null)
                {
                    await installer.InstallPackage(package, flags);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.InstallPackage(package, flags));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {package}.");
                    }
                }
            }
            catch (Exception)
            {
                throw new PackageInstallerException($"Failed to install {package}. The package may not exist or installation failed.");
            }

            PrintSuccess("installed", package);
        }
    }
}