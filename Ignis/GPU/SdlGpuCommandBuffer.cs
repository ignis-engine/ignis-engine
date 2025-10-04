using System.Runtime.CompilerServices;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuCommandBuffer : IDisposable
{
    public SdlPointer<SDL_GPUCommandBuffer, SdlGpuException> NativeCommandBuffer { get; }

    public SdlGpuDevice Device { get; }

    public SdlGpuCommandBuffer(SdlGpuDevice device)
    {
        NativeCommandBuffer = SDL3.SDL_AcquireGPUCommandBuffer(device);
        Device = device;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Submit()
    {
        IgnisDebug.Assert(!NativeCommandBuffer.IsNull, "Command buffer is null.");

        SDL3.SDL_SubmitGPUCommandBuffer(NativeCommandBuffer);
        NativeCommandBuffer.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuTexture WaitAndAcquireSwapchainTexture(SdlWindow window)
    {
        SDL_GPUTexture* swapchainTexture = null;
        uint swapchainWidth = 0;
        uint swapchainHeight = 0;

        if (
            !SDL3.SDL_WaitAndAcquireGPUSwapchainTexture(
                NativeCommandBuffer,
                window.NativeWindow,
                &swapchainTexture,
                &swapchainWidth,
                &swapchainHeight
            )
        )
            throw new SdlGpuException();

        var format = SDL3.SDL_GetGPUSwapchainTextureFormat(Device.NativeDevice, window.NativeWindow);
        return new SdlGpuTexture(Device, swapchainTexture, swapchainWidth, swapchainHeight, format);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (NativeCommandBuffer.IsNull)
            return;

        Submit();
    }
}
