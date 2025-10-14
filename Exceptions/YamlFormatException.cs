namespace setupme.Exceptions
{
    /// <summary>
    /// Thrown when the YAML configuration file is malformed.
    /// </summary>
    public class YamlFormatException : Exception
    {
        public YamlFormatException(string message) : base(message) { }
    }
}
