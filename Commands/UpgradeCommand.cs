using Cocona;
using setupme.Commands;
using setupme.Entities;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class UpgradeCommand : PackageCommandBase
    {
        public UpgradeCommand(IEnumerable<IPackageInstaller> installers) : base(installers) { }

        [Command("update", Description = "Upgrades an installed package to the latest available version.")]
        public async Task UpgradeAsync(
            [Argument(Description = "The name of the package to upgrade.")] string package,
            [Option('v', Description = "Specify a target version to upgrade to. If omitted, the latest version is used.")] string? version = null,
            [Option('f', Description = "Force the upgrade, even if the package is already up to date.")] bool force = false,
            [Option('s', Description = "Specify the package source (e.g. 'winget' or 'choco'). If omitted, all sources are tried.")] string? source = null,
            [Option('q', Description = "Run in quiet mode with minimal output.")] bool quiet = false,
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
                Confirm = yes,
            };

            try
            { 
                if (installer != null)
                {
                    await installer.UpgradePackage(package, flags);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.UpgradePackage(package, flags));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {package}.");
                    }
                }
            }
            catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {package}, the package is non-existing or the installer failed the uninstall process");
            }

            PrintSuccess("upgraded", package);
        }
    }
}