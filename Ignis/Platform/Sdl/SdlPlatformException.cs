using SDL;

namespace Ignis.Platform.Sdl;

internal class SdlPlatformException : PlatformException
{
    public SdlPlatformException()
        : base(SDL3.SDL_GetError() ?? "An unknown SDL Platform error occurred.") { }

    public SdlPlatformException(string message)
        : base(message) { }
}
