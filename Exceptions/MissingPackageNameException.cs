namespace setupme.Exceptions
{
    /// <summary>
    /// Thrown when no package name is specified.
    /// </summary>
    public class MissingPackageNameException : Exception
    {
        public MissingPackageNameException(string message) : base(message) { }
    }
}
