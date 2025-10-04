using Ignis.Math;
using SDL;

namespace Ignis.Platform;

internal unsafe class Window : IDisposable
{
    public SdlPointer<SDL_Window, PlatformException> NativeWindow { get; private set; }

    public Window(string title, Vector2i dimensions)
    {
        NativeWindow = SDL3.SDL_CreateWindow(title, dimensions.X, dimensions.Y, SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        Closing = false;
    }

    public void Dispose()
    {
        SDL3.SDL_DestroyWindow(NativeWindow);
        NativeWindow.Dispose();
    }

    public string Title
    {
        get => SDL3.SDL_GetWindowTitle(NativeWindow) ?? string.Empty;
        set => SDL3.SDL_SetWindowTitle(NativeWindow, value);
    }

    public Vector2i Dimensions
    {
        get
        {
            Vector2i dimensions = new();

            if (!SDL3.SDL_GetWindowSize(NativeWindow, &dimensions.X, &dimensions.Y))
                throw new PlatformException("Failed to get window size");

            return dimensions;
        }
    }

    public bool Closing { get; private set; }

    public void Close()
    {
        Closing = true;
    }

    public static implicit operator SDL_Window*(Window window) => window.NativeWindow;
}
