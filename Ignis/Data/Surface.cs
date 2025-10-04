using Ignis.Platform;
using SDL;

namespace Ignis.Data;

/// <summary>
/// Represents a surface in SDL, which is a two-dimensional image or pixel buffer.
/// </summary>
internal unsafe class Surface : IDisposable
{
    /// <summary>
    /// Gets the pointer to the underlying surface handle.
    /// </summary>
    public SdlPointer<SDL_Surface, DataException> Pointer { get; private set; }
    private readonly bool _deleteOnDispose;

    /// <summary>
    /// Initializes a new instance of the <see cref="Surface"/> class with a given surface pointer.
    /// </summary>
    /// <param name="pointer">The pointer to the surface handle.</param>
    /// <param name="deleteOnDispose">True to delete the surface when disposed; otherwise, false.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="pointer"/> is null.</exception>
    public Surface(SDL_Surface* pointer, bool deleteOnDispose = true)
    {
        Pointer = pointer;
        _deleteOnDispose = deleteOnDispose;
    }

    /// <summary>
    /// Creates a new <see cref="Surface"/> with the specified width, height, and pixel format.
    /// </summary>
    /// <param name="width">The width of the surface in pixels.</param>
    /// <param name="height">The height of the surface in pixels.</param>
    /// <param name="format">The pixel format of the surface.</param>
    /// <returns>A new <see cref="Surface"/> instance.</returns>
    /// <exception cref="DataException">Thrown if the surface cannot be created.</exception>
    public static Surface Create(int width, int height, SDL_PixelFormat format)
    {
        return new Surface(SDL3.SDL_CreateSurface(width, height, format));
    }

    /// <summary>
    /// Creates a new <see cref="Surface"/> from an existing pixel buffer.
    /// </summary>
    /// <param name="width">The width of the surface in pixels.</param>
    /// <param name="height">The height of the surface in pixels.</param>
    /// <param name="format">The pixel format of the surface.</param>
    /// <param name="pixels">Pointer to the pixel buffer.</param>
    /// <param name="pitch">The number of bytes per row of pixel data.</param>
    /// <returns>A new <see cref="Surface"/> instance.</returns>
    /// <exception cref="DataException">Thrown if the surface cannot be created from the pixel buffer.</exception>
    public static Surface CreateFrom(int width, int height, SDL_PixelFormat format, void* pixels, int pitch)
    {
        return new Surface(SDL3.SDL_CreateSurfaceFrom(width, height, format, (IntPtr)pixels, pitch));
    }

    /// <summary>
    /// Loads a BMP image from the specified <see cref="IoStream"/> and creates a <see cref="Surface"/>.
    /// </summary>
    /// <param name="stream">The IOStream to load the BMP image from.</param>
    /// <returns>A new <see cref="Surface"/> instance containing the loaded BMP image.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
    /// <exception cref="DataException">Thrown if the BMP image cannot be loaded.</exception>
    public static Surface LoadBmp(IoStream stream)
    {
        return new Surface(SDL3.SDL_LoadBMP_IO(stream.NativeStream, false));
    }

    /// <summary>
    /// Loads a BMP image from the specified file path and creates a <see cref="Surface"/>.
    /// </summary>
    /// <param name="filePath">The path to the BMP file.</param>
    /// <returns>A new <see cref="Surface"/> instance containing the loaded BMP image.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filePath"/> is null or empty.</exception>
    /// <exception cref="DataException">Thrown if the BMP image cannot be loaded.</exception>
    public static Surface LoadBmp(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.");

        return new Surface(SDL3.SDL_LoadBMP(filePath));
    }

    /// <summary>
    /// Locks the surface for direct pixel access.
    /// </summary>
    /// <exception cref="DataException">Thrown if the surface cannot be locked.</exception>
    public void Lock()
    {
        if (!SDL3.SDL_LockSurface(Pointer))
            throw new DataException();
    }

    /// <summary>
    /// Unlocks the surface after direct pixel access.
    /// </summary>
    public void Unlock()
    {
        SDL3.SDL_UnlockSurface(Pointer);
    }

    /// <summary>
    /// Saves the surface as a BMP image to the specified <see cref="IOStream"/>.
    /// </summary>
    /// <param name="stream">The IOStream to save the BMP image to.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null.</exception>
    /// <exception cref="DataException">Thrown if the surface cannot be saved.</exception>
    public void SaveBMP(IoStream stream)
    {
        if (!SDL3.SDL_SaveBMP_IO(Pointer, stream.NativeStream, false))
            throw new DataException();
    }

    /// <summary>
    /// Saves the surface as a BMP image to the specified file path.
    /// </summary>
    /// <param name="filePath">The path to save the BMP file to.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filePath"/> is null or empty.</exception>
    /// <exception cref="DataException">Thrown if the surface cannot be saved.</exception>
    public void SaveBMP(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty.");

        if (!SDL3.SDL_SaveBMP(Pointer, filePath))
            throw new DataException();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_deleteOnDispose)
            SDL3.SDL_DestroySurface(Pointer);

        Pointer = null;
    }
}
