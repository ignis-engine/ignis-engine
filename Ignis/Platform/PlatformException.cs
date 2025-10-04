using SDL;

namespace Ignis.Platform;

internal class PlatformException : IgnisException
{
    public PlatformException()
        : base(SDL3.SDL_GetError() ?? "An unknown platform error occurred.") { }

    public PlatformException(string message)
        : base(message) { }
}
