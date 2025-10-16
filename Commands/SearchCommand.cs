using Cocona;
using setupme.Commands;
using setupme.Exceptions;
using SetupMe.Interfaces;

namespace SetupMe.Commands
{
    public class SearchCommand : PackageCommandBase
    {
        public SearchCommand(IEnumerable<IPackageInstaller> installers) : base(installers) { }

        public async Task SearchAsync(
            [Argument(Description = "Package name to unistall")] string? packageName,
            [Option('p', Description = "Specify package name")] string? package,
            [Option('s', Description = "Package source (winget/choco)")] string? source
        )
        {
            var pkg = GetPackageName(packageName, package);
            var installer = GetInstallerBySource(source);

            try
            {
                if (installer != null)
                {
                    await installer.SearchPackage(pkg);
                }
                else
                {
                    var success = await TryAllInstallersAsync(i => i.SearchPackage(pkg));
                    if (!success)
                    {
                        throw new PackageInstallerException($"No installer could install {pkg}.");
                    }
                }
            }
            catch (Exception)
            {
                throw new PackageInstallerException($"Failed to uninstall {pkg}, the package is non-existing or the installer failed the uninstall process");
            }

            PrintSuccess("upgraded", pkg);
        }
    }
}