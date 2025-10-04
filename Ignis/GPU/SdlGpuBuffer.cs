using System.Runtime.CompilerServices;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuBuffer : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUBuffer, SdlGpuException> NativeBuffer { get; }

    public SdlGpuDevice Device { get; }

    public uint Size { get; }

    public string DebugLabel
    {
        set => SDL3.SDL_SetGPUBufferName(Device, NativeBuffer, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuBuffer(SdlGpuDevice device, SDL_GPUBufferUsageFlags usage, uint size, string? debugLabel = null)
    {
        Device = device;
        Size = size;

        var info = new SDL_GPUBufferCreateInfo
        {
            size = size,
            usage = usage,
            props = 0,
        };

        NativeBuffer = SDL3.SDL_CreateGPUBuffer(device, &info);

        if (debugLabel is not null)
            DebugLabel = debugLabel;
    }

    public void Dispose()
    {
        SDL3.SDL_ReleaseGPUBuffer(Device, NativeBuffer);
        NativeBuffer.Dispose();
    }
}
