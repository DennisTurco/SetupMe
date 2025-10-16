using Cocona;
using setupme.Entities;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class UninstallCommand
    {
        private readonly IEnumerable<IPackageInstaller> _installers;

        public UninstallCommand(IEnumerable<IPackageInstaller> installers)
        {
            _installers = installers;
        }

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
            var pkg = packageName ?? package;

            if (string.IsNullOrEmpty(pkg))
            {
                throw new MissingPackageNameException("You must specify a package name (either as argument or --package/-p).");
            }

            var installer = _installers.FirstOrDefault(i =>
                (source == "choco") ||
                (source == "winget"));

            if (installer == null)
            {
                throw new SourceNotFoundException($"Source '{source}' not found.");
            }

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
                if (source != null)
                {
                    await installer.UninstallPackage(pkg, flags);
                }
                else
                {
                    var lastTrySuccedeed = false;
                    foreach (var inst in _installers)
                    {
                        try
                        {
                            await inst.UninstallPackage(pkg, flags);
                            lastTrySuccedeed = true;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    if (!lastTrySuccedeed)
                    {
                        throw new PackageInstallerException($"Tried all possible installers, but there is no {pkg} avaiable, make sure it exists");
                    }
                }
            } catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {pkg}, the package is non-existing or the installer failed the uninstall process");
            }
            

            Console.WriteLine($"Package {pkg} uninstalled succesfully!");
        }
    }
}