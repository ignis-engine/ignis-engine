using SDL;

namespace Ignis.Platform;

/// <summary>
/// Defines an interface for custom IOStream operations.
/// </summary>
public interface IIoStreamInterface
{
    /// <summary>
    /// Gets the size of the stream in bytes.
    /// </summary>
    /// <returns>The size of the stream in bytes.</returns>
    /// <exception cref="Exception">Any exception may be thrown depending on the implementation.</exception>
    public long StreamSize();

    /// <summary>
    /// Seeks to a specified offset in the stream.
    /// </summary>
    /// <param name="offset">The seek offset</param>
    /// <param name="whence">The reference point for the offset.</param>
    /// <returns>The final offset in the data stream, or -1 on error.</returns>
    /// <exception cref="Exception">Any exception may be thrown depending on the implementation.</exception>
    public long StreamSeek(long offset, SDL_IOWhence whence);

    /// <summary>
    /// Reads data from the stream into a buffer.
    /// </summary>
    /// <param name="ptr">The pointer to the buffer to read into.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <returns>A tuple containing the IOStatus and the number of bytes read.</returns>
    /// <exception cref="Exception">Any exception may be thrown depending on the implementation.</exception>
    public ulong StreamRead(IntPtr ptr, ulong size);

    /// <summary>
    /// Writes data from a buffer to the stream.
    /// </summary>
    /// <param name="ptr">The pointer to the buffer to write</param>
    /// <param name="size">The size of the buffer</param>
    /// <returns>A tuple containing the IOStatus and the number of bytes written.</returns>
    /// <exception cref="Exception">Any exception may be thrown depending on the implementation.</exception>
    public ulong StreamWrite(IntPtr ptr, ulong size);

    /// <summary>
    /// Flushes the stream, ensuring all buffered data is written.
    /// </summary>
    /// <returns>A tuple containing the IOStatus and a boolean indicating success.</returns>
    /// <exception cref="Exception">Any exception may be thrown depending on the implementation.</exception>
    public bool StreamFlush();

    /// <summary>
    /// Returns true if the stream has reached the end of file (EOF).
    /// </summary>
    /// <returns>True if the stream is at EOF; otherwise, false.</returns>
    public bool StreamIsEof();

    /// <summary>
    /// Closes the stream and releases any associated resources.
    /// </summary>
    /// <returns>True if the stream was successfully closed; otherwise, false.</returns>
    /// <exception cref="Exception">Any exception may be thrown depending on the implementation.</exception>
    public bool StreamClose();
}
