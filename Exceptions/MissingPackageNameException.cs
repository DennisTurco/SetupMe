namespace setupme.Exceptions
{
    /// <summary>
    /// Thrown when no there is an error during the configuration running.
    /// </summary>
    public class RunConfigurationException : Exception
    {
        public RunConfigurationException(string message) : base(message) { }
    }
}
