using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuCommandBuffer : IDisposable
{
    public SdlPointer<SDL_GPUCommandBuffer, SdlGpuException> NativeCommandBuffer { get; }

    public SdlGpuCommandBuffer(SdlGpuDevice device)
    {
        NativeCommandBuffer = SDL3.SDL_AcquireGPUCommandBuffer(device);
    }

    public void Submit()
    {
        IgnisDebug.Assert(!NativeCommandBuffer.IsNull, "Command buffer is null.");

        SDL3.SDL_SubmitGPUCommandBuffer(NativeCommandBuffer);
        NativeCommandBuffer.Dispose();
    }

    public void Dispose()
    {
        if (NativeCommandBuffer.IsNull)
            return;

        Submit();
    }
}
