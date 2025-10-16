using Cocona;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class SearchCommand
    {
        private readonly IEnumerable<IPackageInstaller> _installers;

        public SearchCommand(IEnumerable<IPackageInstaller> installers)
        {
            _installers = installers;
        }

        public async Task SearchAsync(
            [Argument(Description = "Package name to unistall")] string? packageName,
            [Option('p', Description = "Specify package name")] string? package,
            [Option('s', Description = "Package source (winget/choco)")] string? source
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

            if (installer == null)
            {
                throw new SourceNotFoundException($"Source '{source}' not found.");
            }

            if (source != null)
            {
                await installer.SearchPackage(pkg);
            }
            else
            {
                var lastTrySuccedeed = false;
                foreach (var inst in _installers)
                {
                    try
                    {
                        await inst.SearchPackage(pkg);
                        lastTrySuccedeed = true;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                if (!lastTrySuccedeed)
                {
                    Console.WriteLine($"Tried all possible installers, but there is no { pkg } avaiable");
                }
            }
        }
    }
}