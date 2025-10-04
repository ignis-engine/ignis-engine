// The callback wrappers require a wide Exception catch-all
#pragma warning disable CA1031

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SDL;

namespace Ignis.Platform;

internal static unsafe class IoStreamWrapper
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static long GetSizeWrapper(IntPtr userData)
    {
        var handle = GCHandle.FromIntPtr(userData);

        try
        {
            if (handle.Target is IIoStreamInterface si)
                return si.StreamSize();

            SdlError.Set("Invalid size callback");
            return -1;
        }
        catch (Exception ex)
        {
            SdlError.Set(ex.Message);
            return -1;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static long SeekWrapper(IntPtr userData, long offset, SDL_IOWhence whence)
    {
        var handle = GCHandle.FromIntPtr(userData);

        try
        {
            if (handle.Target is IIoStreamInterface si)
                return si.StreamSeek(offset, whence);

            SdlError.Set("Invalid seek callback");
            return -1;
        }
        catch (Exception ex)
        {
            SdlError.Set(ex.Message);
            return -1;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static UIntPtr ReadWrapper(IntPtr userData, IntPtr ptr, UIntPtr size, SDL_IOStatus* status)
    {
        var handle = GCHandle.FromIntPtr(userData);

        try
        {
            if (handle.Target is IIoStreamInterface si)
            {
                var readLength = (UIntPtr)si.StreamRead(ptr, size);

                if (si.StreamIsEof())
                    *status = SDL_IOStatus.SDL_IO_STATUS_EOF;

                return readLength;
            }

            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            SdlError.Set("Invalid read callback");
            return 0;
        }
        catch (Exception ex)
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            SdlError.Set(ex.Message);
            return 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static UIntPtr WriteWrapper(IntPtr userData, IntPtr ptr, UIntPtr size, SDL_IOStatus* status)
    {
        var handle = GCHandle.FromIntPtr(userData);

        try
        {
            if (handle.Target is IIoStreamInterface si)
            {
                var writeLength = si.StreamWrite(ptr, size);
                return (UIntPtr)writeLength;
            }

            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            SdlError.Set("Invalid write callback");
            return 0;
        }
        catch (Exception ex)
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            SdlError.Set(ex.Message);
            return 0;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static SDLBool FlushWrapper(IntPtr userData, SDL_IOStatus* status)
    {
        var handle = GCHandle.FromIntPtr(userData);

        try
        {
            if (handle.Target is IIoStreamInterface si)
                return si.StreamFlush();

            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            SdlError.Set("Invalid flush callback");
            return false;
        }
        catch (Exception ex)
        {
            *status = SDL_IOStatus.SDL_IO_STATUS_ERROR;
            SdlError.Set(ex.Message);
            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static SDLBool CloseWrapper(IntPtr userData)
    {
        var handle = GCHandle.FromIntPtr(userData);

        try
        {
            if (handle.Target is IIoStreamInterface si)
                return si.StreamClose();

            SdlError.Set("Invalid close callback");
            return false;
        }
        catch (Exception ex)
        {
            SdlError.Set(ex.Message);
            return false;
        }
    }
}
