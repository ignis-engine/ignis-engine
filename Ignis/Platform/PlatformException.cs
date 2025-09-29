namespace Ignis.Platform;

/// <summary>
/// Thrown when an error occurred in the Ignis platform system.
/// </summary>
public class PlatformException : IgnisException
{
    /// <summary>
    /// Constructor
    /// </summary>
    public PlatformException() { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The message of the exception</param>
    public PlatformException(string message)
        : base(message) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The message of the exception</param>
    /// <param name="innerException">An exception that was caught and rethrown as an Ignis exception</param>
    public PlatformException(string message, Exception innerException)
        : base(message, innerException) { }
}
