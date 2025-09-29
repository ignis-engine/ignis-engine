using System.Diagnostics;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuTransferBuffer : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUTransferBuffer, SdlGpuException> NativeBuffer { get; }

    public SdlGpuDevice Device { get; }

    public uint Size { get; }

    public bool IsMapped { get; private set; }

    public SdlGpuTransferBuffer(SdlGpuDevice device, SDL_GPUTransferBufferUsage usage, uint size)
    {
        Device = device;
        Size = size;
        IsMapped = false;

        var info = new SDL_GPUTransferBufferCreateInfo
        {
            size = size,
            usage = usage,
            props = 0,
        };

        NativeBuffer = SDL3.SDL_CreateGPUTransferBuffer(device, &info);
    }

    public void* Map(bool cycle = false)
    {
        IgnisDebug.Assert(!IsMapped, "Buffer is already mapped.");

        var result = (void*)SDL3.SDL_MapGPUTransferBuffer(Device, NativeBuffer, cycle);

        if (result is null)
            throw new SdlGpuException();

        IsMapped = true;

        return result;
    }

    public void Unmap()
    {
        IgnisDebug.Assert(IsMapped, "Buffer is not mapped.");

        SDL3.SDL_UnmapGPUTransferBuffer(Device, NativeBuffer);
        IsMapped = false;
    }

    public void Copy(byte[] data, int sourceOffset, int destinationOffset, int length)
    {
        IgnisDebug.Assert(sourceOffset >= 0, "Source offset must be non-negative.");
        IgnisDebug.Assert(destinationOffset >= 0, "Destination offset must be non-negative.");
        IgnisDebug.Assert(length >= 0, "Length must be non-negative.");
        IgnisDebug.Assert(sourceOffset + length <= data.Length, "Source range exceeds data array length.");
        IgnisDebug.Assert(destinationOffset + length <= Size, "Destination range exceeds buffer size.");

        fixed (byte* dataPtr = &data[sourceOffset])
        {
            var mappedPtr = Map();
            Buffer.MemoryCopy(dataPtr, (byte*)mappedPtr + destinationOffset, Size - destinationOffset, length);
            Unmap();
        }
    }

    public void Dispose()
    {
        SDL3.SDL_ReleaseGPUTransferBuffer(Device, NativeBuffer);
        NativeBuffer.Dispose();
    }
}
