using System.Text;

namespace Ignis;

internal sealed class ConsoleFileLogger : IConsoleListener, IDisposable
{
    private readonly FileStream _logFile;
    private readonly StreamWriter _writer;

    public ConsoleFileLogger(string logFileName)
    {
        _logFile = new FileStream(logFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
        _writer = new StreamWriter(_logFile, Encoding.UTF8);
    }

    public void OnLogMessage(LogLevel level, string origin, string message)
    {
        _writer.Write($"[{origin}] ");

        switch (level)
        {
            case LogLevel.Warning:
                _writer.Write("[WARNING]: ");
                break;
            case LogLevel.Error:
                _writer.Write("[ERROR]: ");
                break;
            case LogLevel.Debug:
            case LogLevel.Info:
            default:
                break;
        }

        _writer.WriteLine(message);
        _logFile.Flush();
    }

    public void OnFlush()
    {
        _logFile.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
        _logFile.Dispose();
    }
}
