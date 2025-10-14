namespace setupme.Exceptions
{
    /// <summary>
    /// Thrown when there is an error during package installation.
    /// </summary>
    public class PackageInstallerException : Exception
    {
        public PackageInstallerException(string message) : base(message) { }
    }
}
