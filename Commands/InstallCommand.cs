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

        [Command("install", Description = "Install a package")]
        public async Task InstallAsync(
            [Argument(Description = "Package name to install")] string? packageName,
            [Option('p', Description = "Specify package name")] string? package,
            [Option('v', Description = "Package version")] string? version = null,
            [Option('f', Description = "Force reinstall")] bool force = false,
            [Option('s', Description = "Package source (winget/choco)")] string? source = null,
            [Option('q', Description = "Quiet mode (no output)")] bool quiet = false,
            [Option('y', Description = "Confirm all prompts")] bool yes = false
        )
        {
            string pkg = GetPackageName(packageName, package);
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
                    await installer.InstallPackage(pkg, flags);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.InstallPackage(pkg, flags));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {pkg}.");
                    }
                }
            }
            catch (Exception)
            {
                throw new PackageInstallerException($"Failed to install {pkg}. The package may not exist or installation failed.");
            }

            PrintSuccess("installed", pkg);
        }
    }
}