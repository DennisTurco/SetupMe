using Cocona;

namespace SetupMe.Commands
{
    public class InstallCommand
    {
        public InstallCommand() { }

        [Command("install", Description = "Install a package")]
        public async Task InstallAsync(
            [Argument(Description = "Package name to install")] string? packageName,
            [Option('p', Description = "Specify package name")] string? packageOption,
            [Option("v", Description = "Package version")] string? version = null,
            [Option("f", Description = "Force reinstall")] bool force = false,
            [Option("s", Description = "Package source (winget/choco)")] string? source = null,
            [Option("q", Description = "Quite mode (no output)")] bool quite = false
        )
        {
            var pkg = packageName ?? packageOption;

            if (string.IsNullOrEmpty(pkg))
            {
                Console.Error.WriteLine("Error: You must specify a package name (either as argument or --package/-p).");
                return;
            }

            Console.WriteLine($"Package {pkg} installed succesfully!");
        }
    }
}