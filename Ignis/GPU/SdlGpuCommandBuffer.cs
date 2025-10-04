using System.Runtime.CompilerServices;
using Ignis.Platform;
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
    public SdlGpuTexture WaitAndAcquireSwapchainTexture(Window window)
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

    public void PushVertexUniformData(uint index, byte[] data)
    {
        fixed (byte* pData = data)
        {
            SDL3.SDL_PushGPUVertexUniformData(NativeCommandBuffer, index, (IntPtr)pData, (uint)data.Length);
        }
    }

    public void PushVertexUniformData<T>(uint index, T* data)
        where T : unmanaged
    {
        SDL3.SDL_PushGPUVertexUniformData(NativeCommandBuffer, index, (IntPtr)data, (uint)sizeof(T));
    }

    public void PushFragmentUniformData(uint index, byte[] data)
    {
        fixed (byte* pData = data)
        {
            SDL3.SDL_PushGPUFragmentUniformData(NativeCommandBuffer, index, (IntPtr)pData, (uint)data.Length);
        }
    }

    public void PushFragmentUniformData<T>(uint index, T* data)
        where T : unmanaged
    {
        SDL3.SDL_PushGPUFragmentUniformData(NativeCommandBuffer, index, (IntPtr)data, (uint)sizeof(T));
    }

    public void GenerateMipmapsForTexture(SdlGpuTexture texture)
    {
        SDL3.SDL_GenerateMipmapsForGPUTexture(NativeCommandBuffer, texture.NativeTexture);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (NativeCommandBuffer.IsNull)
            return;

        Submit();
    }
}
