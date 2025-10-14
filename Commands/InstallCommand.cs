using Cocona;
using SetupMe.Interfaces;
using setupme.Entities;
using setupme.Exceptions;

namespace SetupMe.Commands
{
    public class InstallCommand
    {
        private readonly IEnumerable<IPackageInstaller> installers;

        public InstallCommand(IEnumerable<IPackageInstaller> installers)
        {
            this.installers = installers;
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

            var installer = installers.FirstOrDefault(i =>
                (source is null or "" or "choco") ||
                (source == "winget"));

            if (installer == null)
            {
                throw new SourceNotFoundException($"Source '{source}' not found.");
            }

            var flags = new Flags
            {
                Version = version,
                Force = force,
                Source = source,
                Quiet = quiet,
                Confirm = yes
            };

            // TODO: if fails and the source is null try using a different installer for every installer we have
            await installer.InstallPackage(pkg, flags);

            Console.WriteLine($"Package {pkg} installed succesfully!");
        }
    }
}