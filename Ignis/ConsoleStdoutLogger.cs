namespace Ignis;

internal sealed class ConsoleStdoutLogger : IConsoleListener
{
    public void OnLogMessage(LogLevel level, string origin, string message)
    {
        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.Write($"[{origin}] ");

        switch (level)
        {
            case LogLevel.Debug:
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                break;
            case LogLevel.Info:
                System.Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogLevel.Warning:
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.Write("[WARNING]: ");
                break;
            case LogLevel.Error:
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.Write("[ERROR]: ");
                break;
            default:
                System.Console.ForegroundColor = System.Console.ForegroundColor;
                break;
        }

        System.Console.WriteLine(message);
        System.Console.ForegroundColor = System.Console.ForegroundColor;
    }

    public void OnFlush()
    {
        System.Console.Out.Flush();
    }
}
