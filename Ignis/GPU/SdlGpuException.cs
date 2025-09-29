using SDL;

namespace Ignis.GPU;

internal class SdlGpuException : IgnisException
{
    public SdlGpuException()
        : base(SDL3.SDL_GetError() ?? "An unknown SDL GPU error occurred.") { }

    public SdlGpuException(string message)
        : base(message) { }
}
