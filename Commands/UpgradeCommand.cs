using Cocona;
using setupme.Entities;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class UpgradeCommand
    {
        private readonly IEnumerable<IPackageInstaller> _installers;

        public UpgradeCommand(IEnumerable<IPackageInstaller> installers)
        {
            _installers = installers;
        }

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
            var pkg = packageName ?? package;

            if (pkg == null)
            {
                throw new MissingPackageNameException("You must specify a package name (either as argument or --package/-p).");
            }

            var installer = _installers.FirstOrDefault(i =>
                (source == "choco") ||
                (source == "winget"));

            var flags = new Flags
            {
                Version = version ?? string.Empty,
                Force = force,
                Source = source ?? string.Empty,
                Quiet = quiet,
                Confirm = yes,
            };

            if (installer != null)
            {
                await installer.UpgradePackage(pkg, flags);
            }
            else
            {
                var lastTrySuccedeed = false;
                foreach (var inst in _installers)
                {
                    try
                    {
                        await inst.UpgradePackage(pkg, flags);
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

            Console.WriteLine($"Package {pkg} upgraded succesfully!");
        }
    }
}