using setupme.Entities;

namespace SetupMe.Interfaces
{
    public interface IPackageInstaller
    {
        public Task InstallPackage(string packageName, Flags flags);
    }
}
