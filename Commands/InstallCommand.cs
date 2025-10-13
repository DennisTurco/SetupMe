using Cocona;
using SetupMe.Interfaces;

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
                Console.Error.WriteLine("Error: You must specify a package name (either as argument or --package/-p).");
                return;
            }

            var installer = installers.FirstOrDefault(i =>
                (source is null or "" or "choco") ||
                (source == "winget"));

            if (installer is null)
            {
                Console.Error.WriteLine($"Error: Source '{source}' not found.");
                return;
            }

            await installer.InstallPackage(pkg, version, force, quiet, yes);

            Console.WriteLine($"Package {pkg} installed succesfully!");
        }
    }
}