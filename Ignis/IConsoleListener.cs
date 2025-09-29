namespace Ignis;

/// <summary>
/// Interface for receiving console log messages.
/// </summary>
public interface IConsoleListener
{
    /// <summary>
    /// Called when a message is logged to the console at or above the current log level.
    /// </summary>
    /// <param name="level">The log level of the message</param>
    /// <param name="origin">The origin of the log message (subsystem, component)</param>
    /// <param name="message">The message</param>
    public void OnLogMessage(LogLevel level, string origin, string message);

    /// <summary>
    /// Called when the console is flushed (usually on a crash). When implementing this function ensure that
    /// all log messages are flushed properly to whatever output is used.
    /// </summary>
    public void OnFlush();
}
