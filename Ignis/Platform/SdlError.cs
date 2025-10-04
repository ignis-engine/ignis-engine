using Ignis.Interop;
using SDL;

namespace Ignis.Platform;

public static class SdlError
{
    public static string Get()
    {
        return SDL3.SDL_GetError() ?? "Unknown SDL error";
    }

    public static unsafe void Set(string message)
    {
        using var msgBytes = Utf8Interop.Utf8EncodeHeap(message);
        SDL3.SDL_SetError(msgBytes, __arglist());
    }
}
