using Cocona;
using setupme.Commands;
using setupme.Entities;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class UninstallCommand : PackageCommandBase
    {
        public UninstallCommand(IEnumerable<IPackageInstaller> installers) : base(installers) { }

        [Command("uninstall", Description = "Removes a previously installed package from the system.")]
        public async Task UninstallAsync(
            [Argument(Description = "The name of the package to uninstall.")] string package,
            [Option('v', Description = "Specify the package version to uninstall (if multiple versions are installed).")] string? version = null,
            [Option('a', Description = "Uninstall all versions of the package (default: false).")] bool all = false,
            [Option('f', Description = "Force the uninstallation, ignoring dependency or safety checks.")] bool force = false,
            [Option('s', Description = "Specify the package source (e.g. 'winget' or 'choco'). If omitted, all sources are tried.")] string? source = null,
            [Option('q', Description = "Run in quiet mode with minimal output.")] bool quiet = false,
            [Option('y', Description = "Automatically confirm all prompts and proceed without asking.")] bool yes = false
        )
        {
            var installer = GetInstallerBySource(source);

            var flags = new Flags
            {
                Version = version ?? string.Empty,
                AllVersions = all,
                Force = force,
                Source = source ?? string.Empty,
                Quiet = quiet,
                Confirm = yes
            };

            try
            {
                if (installer != null)
                {
                    await installer.UninstallPackage(package, flags);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.UninstallPackage(package, flags));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {package}.");
                    }
                }
            } catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {package}, the package is non-existing or the installer failed the uninstall process");
            }

            PrintSuccess("uninstalled", package);
        }
    }
}