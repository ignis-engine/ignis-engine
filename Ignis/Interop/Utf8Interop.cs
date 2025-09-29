using System.Text;

namespace Ignis.Interop;

/// <summary>
/// Various helper functions for native interaction with UTF-8 strings.
/// </summary>
public static class Utf8Interop
{
    /// <summary>
    /// Compute the maximum length a string could potentially have when converted to UTF8.
    /// This can be used for native buffer allocations.
    /// </summary>
    public static int ComputeWorstCaseUtf8Length(string? str)
    {
        if (str is null)
            return 0;

        return str.Length * 4 + 1;
    }

    /// <summary>
    /// Convert a given string to an UTF-8 native string buffer.
    /// The given bufferSize must be large enough to contain the entire string.
    /// You can use <see cref="ComputeWorstCaseUtf8Length(string?)"/> for this.
    /// </summary>
    /// <returns>The given buffer pointer or 0 if the given string is null.</returns>
    public static unsafe byte* Utf8EncodeHeap(string? str, byte* buffer, int bufferSize)
    {
        if (str is null)
            return (byte*)IntPtr.Zero;

        fixed (char* strPtr = str)
        {
            Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer, bufferSize);
        }

        return buffer;
    }

    /// <summary>
    /// Allocate a buffer on the heap, convert a given string to UTF8 and write it to this buffer.
    /// The returned native heap must be disposed of when no longer needed.
    /// </summary>
    /// <param name="str">The string</param>
    /// <returns>A native heap containing the string</returns>
    public static unsafe NativeHeap Utf8EncodeHeap(string? str)
    {
        if (str is null)
            return new NativeHeap();

        var bufferSize = ComputeWorstCaseUtf8Length(str);
        var buffer = NativeHeap.Allocate(bufferSize);

        fixed (char* strPtr = str)
        {
            Encoding.UTF8.GetBytes(strPtr, str.Length + 1, buffer.HeapPtr, bufferSize);
        }

        return buffer;
    }

    /// <summary>
    /// Determine a string length in C-style (count bytes until the '\0' terminator)
    /// </summary>
    public static unsafe int StrLen(IntPtr buffer)
    {
        if (buffer == IntPtr.Zero)
            return 0;

        var ptr = (byte*)buffer;
        while (*ptr != 0)
        {
            ptr++;
        }

        return (int)(ptr - (byte*)buffer);
    }

    /// <summary>
    /// Convert a given pointer to a string.
    /// Warning: does not free the given buffer. You must do this yourself.
    /// </summary>
    public static unsafe string? Utf8Decode(IntPtr s)
    {
        if (s == IntPtr.Zero)
            return null;

        var length = StrLen(s);
        return Encoding.UTF8.GetString((byte*)s, length);
    }
}
