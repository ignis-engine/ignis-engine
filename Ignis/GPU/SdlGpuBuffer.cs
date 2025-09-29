using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuBuffer : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUBuffer, SdlGpuException> NativeBuffer { get; }

    public SdlGpuDevice Device { get; }

    public uint Size { get; }

    public SdlGpuBuffer(SdlGpuDevice device, SDL_GPUBufferUsageFlags usage, uint size)
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
    }

    public void Dispose()
    {
        SDL3.SDL_ReleaseGPUBuffer(Device, NativeBuffer);
        NativeBuffer.Dispose();
    }
}
