using System.Runtime.InteropServices;
using Ignis.Interop;
using SDL;

namespace Ignis.Platform;

/// <summary>
/// Represents an IOStream, which is a stream for reading and writing data.
/// </summary>
internal unsafe class IoStream : IDisposable
{
    /// <summary>
    /// Gets the native pointer to the IOStream handle.
    /// </summary>
    public SDL_IOStream* NativeStream { get; private set; }

    /// <summary>
    /// Custom stream interface for handling specific stream operations.
    /// When the default SDL stream operations are used, this will be null.
    /// </summary>
    public IIoStreamInterface? StreamInterface { get; private set; }

    private GCHandle? _gcHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="IoStream"/> class with a native IOStream handle.
    /// </summary>
    /// <param name="handle">The native IOStream handle.</param>
    /// <param name="streamInterface">Optional interface for custom stream operations.</param>
    /// <param name="gcHandle">Optional garbage collection handle for the stream interface. It will be freed in dispose.</param>
    /// <exception cref="PlatformException">Thrown if the handle is null.</exception>
    private IoStream(SDL_IOStream* handle, IIoStreamInterface? streamInterface, GCHandle? gcHandle)
    {
        if (handle is null)
            throw new PlatformException("IOStream handle cannot be null.");

        NativeStream = handle;
        StreamInterface = streamInterface;
        _gcHandle = gcHandle;
    }

    /// <summary>
    /// Creates an <see cref="IoStream"/> from a file with the specified path and mode.
    /// </summary>
    /// <param name="path">The path to the file to open.</param>
    /// <param name="mode">The file mode (e.g., "rb", "wb").</param>
    /// <returns>A new <see cref="IoStream"/> instance for the file.</returns>
    /// <exception cref="PlatformException">Thrown if the file cannot be opened.</exception>
    public static IoStream FromFile(string path, string mode)
    {
        var result = SDL3.SDL_IOFromFile(path, mode);

        if (result is null)
            throw new PlatformException();

        return new IoStream(result, null, null);
    }

    /// <summary>
    /// Creates an <see cref="IoStream"/> from a memory buffer.
    /// </summary>
    /// <param name="mem">Pointer to the memory buffer.</param>
    /// <param name="size">Size of the memory buffer in bytes.</param>
    /// <returns>A new <see cref="IoStream"/> instance for the memory buffer.</returns>
    /// <exception cref="PlatformException">Thrown if the stream cannot be created.</exception>
    public static IoStream FromMemory(void* mem, nuint size)
    {
        var result = SDL3.SDL_IOFromMem((IntPtr)mem, size);

        if (result is null)
            throw new PlatformException();

        return new IoStream(result, null, null);
    }

    /// <summary>
    /// Creates an <see cref="IoStream"/> backed by dynamic memory.
    /// </summary>
    /// <returns>A new <see cref="IoStream"/> instance backed by dynamic memory.</returns>
    /// <exception cref="PlatformException">Thrown if the stream cannot be created.</exception>
    public static IoStream FromDynamicMemory()
    {
        var result = SDL3.SDL_IOFromDynamicMem();

        if (result is null)
            throw new PlatformException();

        return new IoStream(result, null, null);
    }

    /// <summary>
    /// Creates an <see cref="IoStream"/> from a custom stream interface.
    /// </summary>
    /// <param name="streamInterface">An instance of <see cref="IIoStreamInterface"/> that defines custom stream operations.</param>
    /// <returns>A new <see cref="IoStream"/> instance that uses the provided stream interface.</returns>
    /// <exception cref="PlatformException">Thrown if the IOStream cannot be created from the interface.</exception>
    public static IoStream FromStreamInterface(IIoStreamInterface streamInterface)
    {
        var iOStreamInterface = new SDL_IOStreamInterface
        {
            version = (uint)sizeof(SDL_IOStreamInterface),
            size = &IoStreamWrapper.GetSizeWrapper,
            seek = &IoStreamWrapper.SeekWrapper,
            read = &IoStreamWrapper.ReadWrapper,
            write = &IoStreamWrapper.WriteWrapper,
            flush = &IoStreamWrapper.FlushWrapper,
            close = &IoStreamWrapper.CloseWrapper,
        };

        var handle = GCHandle.Alloc(streamInterface);

        var result = SDL3.SDL_OpenIO(&iOStreamInterface, GCHandle.ToIntPtr(handle));

        if (result is null)
        {
            handle.Free();
            throw new PlatformException();
        }

        return new IoStream(result, streamInterface, handle);
    }

    /// <summary>
    /// Gets the current status of the IOStream.
    /// </summary>
    /// <returns>The <see cref="SDL_IOStatus"/> of the stream.</returns>
    public SDL_IOStatus GetStatus()
    {
        return SDL3.SDL_GetIOStatus(NativeStream);
    }

    /// <summary>
    /// Gets the size of the IOStream in bytes.
    /// </summary>
    /// <returns>The size of the stream in bytes.</returns>
    public long GetSize()
    {
        return SDL3.SDL_GetIOSize(NativeStream);
    }

    /// <summary>
    /// Seeks to a specified offset in the stream.
    /// </summary>
    /// <param name="offset">The offset to seek to.</param>
    /// <param name="whence">The reference point for the offset.</param>
    /// <returns>The new position in the stream.</returns>
    public long Seek(long offset, SDL_IOWhence whence)
    {
        return SDL3.SDL_SeekIO(NativeStream, offset, whence);
    }

    /// <summary>
    /// Gets the current position in the stream.
    /// </summary>
    /// <returns>The current position in the stream.</returns>
    /// <exception cref="PlatformException">Thrown if the position cannot be determined.</exception>
    public long Tell()
    {
        var position = SDL3.SDL_TellIO(NativeStream);

        if (position < 0)
            throw new PlatformException();

        return position;
    }

    /// <summary>
    /// Reads data from the stream into a buffer.
    /// </summary>
    /// <param name="ptr">Pointer to the buffer to read into.</param>
    /// <param name="size">Number of bytes to read.</param>
    /// <returns>The number of bytes read.</returns>
    public ulong Read(IntPtr ptr, ulong size)
    {
        return SDL3.SDL_ReadIO(NativeStream, ptr, (nuint)size);
    }

    /// <summary>
    /// Writes data from a buffer to the stream.
    /// </summary>
    /// <param name="ptr">Pointer to the buffer to write from.</param>
    /// <param name="size">Number of bytes to write.</param>
    /// <returns>The number of bytes written.</returns>
    public ulong Write(IntPtr ptr, ulong size)
    {
        return SDL3.SDL_WriteIO(NativeStream, ptr, (nuint)size);
    }

    /// <summary>
    /// Writes a byte array to the stream.
    /// </summary>
    /// <param name="buffer">Buffer to write</param>
    /// <returns>The number of bytes written.</returns>
    public ulong Write(byte[] buffer)
    {
        if (buffer.Length == 0)
            return 0;

        fixed (byte* ptr = buffer)
        {
            return SDL3.SDL_WriteIO(NativeStream, (IntPtr)ptr, (nuint)buffer.Length);
        }
    }

    /// <summary>
    /// Writes a string to the stream.
    /// </summary>
    /// <param name="str">The string to write.</param>
    /// <returns>The number of bytes written.</returns>
    public ulong Write(string str)
    {
        using var strBytes = Utf8Interop.Utf8EncodeHeap(str);
        return SDL3.SDL_IOprintf(NativeStream, strBytes, __arglist());
    }

    /// <summary>
    /// Flushes the stream, ensuring all buffered data is written.
    /// </summary>
    /// <returns><c>true</c> if the flush was successful; otherwise, <c>false</c>.</returns>
    public bool Flush()
    {
        return SDL3.SDL_FlushIO(NativeStream);
    }

    /// <summary>
    /// Reads an unsigned 8-bit integer from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public byte ReadU8()
    {
        byte value = 0;

        if (!SDL3.SDL_ReadU8(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 8-bit integer from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public sbyte ReadS8()
    {
        sbyte value = 0;

        if (!SDL3.SDL_ReadS8(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer (little-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public ushort ReadU16()
    {
        ushort value = 0;

        if (!SDL3.SDL_ReadU16LE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 16-bit integer (little-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public short ReadS16()
    {
        short value = 0;

        if (!SDL3.SDL_ReadS16LE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads an unsigned 16-bit integer (big-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public ushort ReadU16BE()
    {
        ushort value = 0;

        if (!SDL3.SDL_ReadU16BE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 16-bit integer (big-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public short ReadS16BE()
    {
        short value = 0;

        if (!SDL3.SDL_ReadS16BE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer (little-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public uint ReadU32()
    {
        uint value = 0;

        if (!SDL3.SDL_ReadU32LE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 32-bit integer (little-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public int ReadS32()
    {
        int value = 0;

        if (!SDL3.SDL_ReadS32LE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer (big-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public uint ReadU32BE()
    {
        uint value = 0;

        if (!SDL3.SDL_ReadU32BE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 32-bit integer (big-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public int ReadS32BE()
    {
        int value = 0;

        if (!SDL3.SDL_ReadS32BE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads an unsigned 64-bit integer (little-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public ulong ReadU64()
    {
        ulong value = 0;

        if (!SDL3.SDL_ReadU64LE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 64-bit integer (little-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public long ReadS64()
    {
        long value = 0;

        if (!SDL3.SDL_ReadS64LE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads an unsigned 64-bit integer (big-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public ulong ReadU64BE()
    {
        ulong value = 0;

        if (!SDL3.SDL_ReadU64BE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Reads a signed 64-bit integer (big-endian) from the stream.
    /// </summary>
    /// <returns>The value read.</returns>
    /// <exception cref="PlatformException">Thrown if the value cannot be read.</exception>
    public long ReadS64BE()
    {
        long value = 0;

        if (!SDL3.SDL_ReadS64BE(NativeStream, &value))
            throw new PlatformException();

        return value;
    }

    /// <summary>
    /// Writes an unsigned 8-bit integer to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(byte value)
    {
        if (!SDL3.SDL_WriteU8(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 8-bit integer to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(sbyte value)
    {
        if (!SDL3.SDL_WriteS8(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes an unsigned 16-bit integer (little-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(ushort value)
    {
        if (!SDL3.SDL_WriteU16LE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 16-bit integer (little-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(short value)
    {
        if (!SDL3.SDL_WriteS16LE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes an unsigned 16-bit integer (big-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void WriteBE(ushort value)
    {
        if (!SDL3.SDL_WriteU16BE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 16-bit integer (big-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void WriteBE(short value)
    {
        if (!SDL3.SDL_WriteS16BE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes an unsigned 32-bit integer (little-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(uint value)
    {
        if (!SDL3.SDL_WriteU32LE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 32-bit integer (little-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(int value)
    {
        if (!SDL3.SDL_WriteS32LE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes an unsigned 32-bit integer (big-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void WriteBE(uint value)
    {
        if (!SDL3.SDL_WriteU32BE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 32-bit integer (big-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void WriteBE(int value)
    {
        if (!SDL3.SDL_WriteS32BE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes an unsigned 64-bit integer (little-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(ulong value)
    {
        if (!SDL3.SDL_WriteU64LE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 64-bit integer (little-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void Write(long value)
    {
        if (!SDL3.SDL_WriteS64LE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes an unsigned 64-bit integer (big-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void WriteBE(ulong value)
    {
        if (!SDL3.SDL_WriteU64BE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Writes a signed 64-bit integer (big-endian) to the stream.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="PlatformException">Thrown if the value cannot be written.</exception>
    public void WriteBE(long value)
    {
        if (!SDL3.SDL_WriteS64BE(NativeStream, value))
            throw new PlatformException();
    }

    /// <summary>
    /// Closes the IOStream and releases associated resources.
    /// </summary>
    /// <exception cref="PlatformException">Thrown if the stream cannot be closed.</exception>
    public void Close()
    {
        if (!SDL3.SDL_CloseIO(NativeStream))
            throw new PlatformException();

        StreamInterface = null;
        NativeStream = null;

        _gcHandle?.Free();
        _gcHandle = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Close();
    }
}
