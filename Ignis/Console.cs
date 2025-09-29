namespace Ignis;

public static class Console
{
#if DEBUG || IGNIS_ENABLE_ASSERTS
    private static LogLevel _currentLogLevel = LogLevel.Debug;
#else
    private static LogLevel _currentLogLevel = LogLevel.Info;
#endif

    private static readonly HashSet<IConsoleListener> Listeners = new();

    /// <summary>
    /// Set the log level for the console. Messages below this level will not be logged.
    /// In Debug the default is Debug, in Release the default is Info.
    /// </summary>
    /// <param name="level"></param>
    public static void SetLogLevel(LogLevel level)
    {
        _currentLogLevel = level;
    }

    /// <summary>
    /// Add a listener to the console. This listener will receive all messages equal
    /// or above the current log level.
    /// </summary>
    /// <param name="listener">The listener to add</param>
    public static void AddListener(IConsoleListener listener)
    {
        Listeners.Add(listener);
    }

    /// <summary>
    /// Remove a listener from the console. The listener will no longer receive messages.
    /// </summary>
    /// <param name="listener">The listener to remove</param>
    public static void RemoveListener(IConsoleListener listener)
    {
        Listeners.Remove(listener);
    }

    /// <summary>
    /// Log a message to the console with <see cref="LogLevel.Info"/>.
    /// This is a variadic function that accepts a format string and arguments.
    /// For example:
    ///     Console.Log("MyComponent", "Hello {0}", "World");
    /// </summary>
    /// <param name="origin">The origin of the log message (subsystem, component)</param>
    /// <param name="message">Format string or message</param>
    /// <param name="args">Optional arguments for string formatting</param>
    public static void Log(string origin, string message, params object[] args)
    {
        LogInfo(origin, message, args);
    }

    /// <summary>
    /// Log a message to the console with <see cref="LogLevel.Debug"/>.
    /// This is a variadic function that accepts a format string and arguments.
    /// For example:
    ///     Console.Log("MyComponent", "Hello {0}", "World");
    /// </summary>
    /// <param name="origin">The origin of the log message (subsystem, component)</param>
    /// <param name="message">Format string or message</param>
    /// <param name="args">Optional arguments for string formatting</param>
    public static void LogDebug(string origin, string message, params object[] args)
    {
        LogInternal(LogLevel.Debug, origin, message, args);
    }

    /// <summary>
    /// Log a message to the console with <see cref="LogLevel.Info"/>.
    /// This is a variadic function that accepts a format string and arguments.
    /// For example:
    ///     Console.Log("MyComponent", "Hello {0}", "World");
    /// </summary>
    /// <param name="origin">The origin of the log message (subsystem, component)</param>
    /// <param name="message">Format string or message</param>
    /// <param name="args">Optional arguments for string formatting</param>
    public static void LogInfo(string origin, string message, params object[] args)
    {
        LogInternal(LogLevel.Info, origin, message, args);
    }

    /// <summary>
    /// Log a message to the console with <see cref="LogLevel.Warning"/>.
    /// This is a variadic function that accepts a format string and arguments.
    /// For example:
    ///     Console.Log("MyComponent", "Hello {0}", "World");
    /// </summary>
    /// <param name="origin">The origin of the log message (subsystem, component)</param>
    /// <param name="message">Format string or message</param>
    /// <param name="args">Optional arguments for string formatting</param>
    public static void LogWarning(string origin, string message, params object[] args)
    {
        LogInternal(LogLevel.Warning, origin, message, args);
    }

    /// <summary>
    /// Log a message to the console with <see cref="LogLevel.Error"/>.
    /// This is a variadic function that accepts a format string and arguments.
    /// For example:
    ///     Console.Log("MyComponent", "Hello {0}", "World");
    /// </summary>
    /// <param name="origin">The origin of the log message (subsystem, component)</param>
    /// <param name="message">Format string or message</param>
    /// <param name="args">Optional arguments for string formatting</param>
    public static void LogError(string origin, string message, params object[] args)
    {
        LogInternal(LogLevel.Error, origin, message, args);
    }

    /// <summary>
    /// Flush the console messages. This can be used to force the console to write all messages to the listeners
    /// in case of a crash. This ensures that log files are fully written on unexpected shutdown.
    /// </summary>
    public static void Flush()
    {
        foreach (var listener in Listeners)
            listener.OnFlush();
    }

    private static void LogInternal(LogLevel level, string origin, string message, params object[] args)
    {
        if (_currentLogLevel > level)
            return;

        foreach (var listener in Listeners)
            listener.OnLogMessage(level, origin, string.Format(message, args));
    }
}
