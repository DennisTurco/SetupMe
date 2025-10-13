namespace SetupMe.Interfaces
{
    public interface IPackageInstaller
    {
        public Task InstallPackage(string packageName, string? version, bool force, bool quiet, bool autoConfirm);
    }
}
