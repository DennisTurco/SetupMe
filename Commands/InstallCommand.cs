using Cocona;
using SetupMe.Interfaces;
using setupme.Entities;
using setupme.Exceptions;

namespace SetupMe.Commands
{
    public class InstallCommand
    {
        private readonly IEnumerable<IPackageInstaller> _installers;

        public InstallCommand(IEnumerable<IPackageInstaller> installers)
        {
            _installers = installers;
        }

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
                Force = force,
                Source = source ?? string.Empty,
                Quiet = quiet,
                Confirm = yes
            };

            try
            {
                if (source != null)
                {
                    await installer.InstallPackage(pkg, flags);
                }
                else
                {
                    var lastTrySuccedeed = false;
                    foreach (var inst in _installers)
                    {
                        try
                        {
                            await inst.InstallPackage(pkg, flags);
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
                throw new PackageInstallerException($"Failed to install {pkg}, the package is non-existing or the installer failed the install process");
            }

            Console.WriteLine($"Package {pkg} installed succesfully!");
        }
    }
}