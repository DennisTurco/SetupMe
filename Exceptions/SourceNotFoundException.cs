namespace setupme.Exceptions
{
    /// <summary>
    /// Thrown when a given source cannot be found.
    /// </summary>
    public class SourceNotFoundException : Exception
    {
        public SourceNotFoundException(string message) : base(message) { }
    }
}
