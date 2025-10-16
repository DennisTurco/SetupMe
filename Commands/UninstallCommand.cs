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

        [Command("uninstall", Description = "Uninstall a package")]
        public async Task UninstallAsync(
            [Argument(Description = "Package name to unistall")] string? packageName,
            [Option('p', Description = "Specify package name")] string? package,
            [Option('v', Description = "Package version")] string? version = null,
            [Option('a', Description = "All version, default false")] bool all = false,
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
                    await installer.UninstallPackage(pkg, flags);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.UninstallPackage(pkg, flags));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {pkg}.");
                    }
                }
            } catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {pkg}, the package is non-existing or the installer failed the uninstall process");
            }

            PrintSuccess("uninstalled", pkg);
        }
    }
}