using System.Runtime.CompilerServices;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuCopyPass : IDisposable
{
    public SdlPointer<SDL_GPUCopyPass, SdlGpuException> NativeCopyPass { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuCopyPass(SdlGpuCommandBuffer commandBuffer)
    {
        NativeCopyPass = SDL3.SDL_BeginGPUCopyPass(commandBuffer.NativeCommandBuffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadBuffer(SdlGpuTransferBuffer transferBuffer, SdlGpuBuffer targetBuffer)
    {
        IgnisDebug.Assert(transferBuffer.Size <= targetBuffer.Size, "Transfer buffer size exceeds target buffer size.");
        UploadBuffer(transferBuffer, 0, targetBuffer, 0, transferBuffer.Size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadBuffer(
        SdlGpuTransferBuffer transferBuffer,
        SdlGpuBuffer targetBuffer,
        uint size,
        bool cycle = false
    )
    {
        IgnisDebug.Assert(size <= transferBuffer.Size, "Size exceeds transfer buffer size.");
        IgnisDebug.Assert(size <= targetBuffer.Size, "Size exceeds target buffer size.");
        IgnisDebug.Assert(size > 0, "Size must be greater than zero.");

        UploadBuffer(transferBuffer, 0, targetBuffer, 0, size, cycle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadBuffer(
        SdlGpuTransferBuffer transferBuffer,
        uint sourceOffset,
        SdlGpuBuffer targetBuffer,
        uint targetOffset,
        uint size,
        bool cycle = false
    )
    {
        IgnisDebug.Assert(sourceOffset + size <= transferBuffer.Size, "Source range exceeds transfer buffer size.");
        IgnisDebug.Assert(targetOffset + size <= targetBuffer.Size, "Target range exceeds target buffer size.");
        IgnisDebug.Assert(size > 0, "Size must be greater than zero.");

        var src = new SDL_GPUTransferBufferLocation
        {
            transfer_buffer = transferBuffer.NativeBuffer,
            offset = sourceOffset,
        };

        var dst = new SDL_GPUBufferRegion
        {
            buffer = targetBuffer.NativeBuffer,
            offset = targetOffset,
            size = size,
        };

        SDL3.SDL_UploadToGPUBuffer(NativeCopyPass, &src, &dst, cycle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UploadTexture(SdlGpuTransferBuffer transferBuffer, SdlGpuTexture targetTexture, bool cycle = false)
    {
        var src = new SDL_GPUTextureTransferInfo { transfer_buffer = transferBuffer.NativeBuffer, offset = 0 };

        var dst = new SDL_GPUTextureRegion
        {
            texture = targetTexture.NativeTexture,
            x = 0,
            y = 0,
            w = targetTexture.Width,
            h = targetTexture.Height,
            d = 1,
        };

        SDL3.SDL_UploadToGPUTexture(NativeCopyPass, &src, &dst, cycle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        SDL3.SDL_EndGPUCopyPass(NativeCopyPass);
    }
}
