using SDL;

namespace Ignis.Data;

internal class DataException : IgnisException
{
    public DataException()
        : base(SDL3.SDL_GetError() ?? "An unknown platform error occurred.") { }

    public DataException(string message)
        : base(message) { }
}
