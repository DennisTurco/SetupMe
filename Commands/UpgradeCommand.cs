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

        [Command("update", Description = "Update a package")]
        public async Task UpgradeAsync(
            [Argument(Description = "Package name")] string? packageName,
            [Option('p', Description = "Specify package name")] string? package,
            [Option('v', Description = "Package version")] string? version = null,
            [Option('f', Description = "Force the behaviour")] bool force = false,
            [Option('s', Description = "Package source (winget/choco)")] string? source = null,
            [Option('q', Description = "Quiet mode (no output)")] bool quiet = false,
            [Option('y', Description = "Confirm all prompts")] bool yes = false
        )
        {
            var pkg = GetPackageName(packageName, package);
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
                    await installer.UpgradePackage(pkg, flags);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.UpgradePackage(pkg, flags));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {pkg}.");
                    }
                }
            }
            catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {pkg}, the package is non-existing or the installer failed the uninstall process");
            }

            PrintSuccess("upgraded", pkg);
        }
    }
}