using setupme.Exceptions;
using SetupMe.Interfaces;

namespace setupme.Commands
{
    public abstract class PackageCommandBase
    {
        protected readonly IEnumerable<IPackageInstaller> Installers;

        protected PackageCommandBase(IEnumerable<IPackageInstaller> installers)
        {
            Installers = installers;
        }

        protected string GetPackageName(string? packageName, string? packageOption)
        {
            var pkg = packageName ?? packageOption;
            if (string.IsNullOrEmpty(pkg))
            {
                throw new MissingPackageNameException("You must specify a package name (either as argument or --package/-p).");
            }
            return pkg;
        }

        protected IPackageInstaller? GetInstallerBySource(string? source)
        {
            if (string.IsNullOrEmpty(source)) return null;
            return Installers.FirstOrDefault(i =>
                source.Equals("choco", StringComparison.OrdinalIgnoreCase) ||
                source.Equals("winget", StringComparison.OrdinalIgnoreCase)
            );
        }

        protected async Task<bool> TryAllInstallersAsync(Func<IPackageInstaller, Task> action)
        {
            var success = false;
            foreach (var installer in Installers)
            {
                try
                {
                    await action(installer);
                    success = true;
                }
                catch
                {
                    // next one
                    continue;
                }
            }
            return success;
        }

        protected void PrintSuccess(string action, string pkg)
        {
            Console.WriteLine($"Package {pkg} {action} successfully!");
        }
    }
}
