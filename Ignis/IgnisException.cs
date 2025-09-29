namespace Ignis;

/// <summary>
/// An exception thrown by a component of the Ignis engine
/// </summary>
public class IgnisException : Exception
{
    private const string LogOrigin = "Exception";

    /// <summary>
    /// Constructor
    /// </summary>
    public IgnisException()
    {
        Console.LogError(LogOrigin, "Ignis exception was thrown.");
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The message of the exception</param>
    public IgnisException(string message)
        : base(message)
    {
        Console.LogError(LogOrigin, "Ignis exception was thrown: {0}", message);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The message of the exception</param>
    /// <param name="innerException">An exception that was caught and rethrown as an Ignis exception</param>
    public IgnisException(string message, Exception innerException)
        : base(message, innerException)
    {
        Console.LogError(LogOrigin, "Ignis exception was thrown: {0}", message);
        Console.LogError(LogOrigin, "Inner exception: {0}", message);
    }
}
