using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Ignis;

public static class IgnisDebug
{
    private const string LogOrigin = "Assert";

    [Conditional("IGNIS_ENABLE_ASSERTS")]
    public static void Assert([DoesNotReturnIf(false)] bool condition, string? message = null)
    {
        if (!condition)
            Fail(message);
    }

    private static void Fail(string? message = null)
    {
        if (message is not null)
            Console.LogError(LogOrigin, "Assertion failed: {0}", message);
        else
            Console.LogError(LogOrigin, "Assertion failed.");

#if IGNIS_JIT_BUILD
        StackDump();
#endif

        Debugger.Break();
    }

#if IGNIS_JIT_BUILD
    [RequiresDynamicCode("Can't do a stack-dump in AOT due to GetMethod")]
    private static void StackDump()
    {
        Console.LogError(LogOrigin, "Stack trace:");

        var t = new StackTrace();
        foreach (var frame in t.GetFrames())
        {
            var method = frame.GetMethod();
            var type = method?.DeclaringType;

            if (type is null)
                continue;

            Console.LogError(LogOrigin, "  at " + type.FullName + "." + method?.Name);
        }

        Console.Flush();
    }
#endif
}
