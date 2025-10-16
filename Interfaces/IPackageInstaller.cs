using setupme.Entities;

namespace SetupMe.Interfaces
{
    public interface IPackageInstaller
    {
        public Task InstallPackage(string packageName, Flags flags);
        public Task UninstallPackage(string packageName, Flags flags);
        public Task UpgradePackage(string packageName, Flags flags);
        public Task SearchPackage(string packageName);
    }
}
