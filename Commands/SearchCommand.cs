using Cocona;
using setupme.Commands;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class SearchCommand : PackageCommandBase
    {
        public SearchCommand(IEnumerable<IPackageInstaller> installers) : base(installers) { }

        [Command("search", Description = "Searches for a package by name across available sources.")]
        public async Task SearchAsync(
            [Argument(Description = "The name of the package to search for.")] string package,
            [Option('s', Description = "Specify the package source (e.g. 'winget' or 'choco'). If omitted, all sources are searched.")] string? source = null
        )
        {
            var installer = GetInstallerBySource(source);

            try
            {
                if (installer != null)
                {
                    await installer.SearchPackage(package);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.SearchPackage(package));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {package}.");
                    }
                }
            }
            catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {package}, the package is non-existing or the installer failed the uninstall process");
            }

            PrintSuccess("upgraded", package);
        }
    }
}