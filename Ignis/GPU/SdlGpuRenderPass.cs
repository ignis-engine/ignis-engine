using System.Runtime.CompilerServices;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuRenderPass : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPURenderPass, SdlGpuException> NativeRenderPass { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuRenderPass(SdlGpuCommandBuffer commandBuffer, SdlGpuTexture targetTexture, SDL_FColor clearColor)
    {
        var info = new SDL_GPUColorTargetInfo
        {
            texture = targetTexture.NativeTexture,
            clear_color = clearColor,
            load_op = SDL_GPULoadOp.SDL_GPU_LOADOP_CLEAR,
            store_op = SDL_GPUStoreOp.SDL_GPU_STOREOP_STORE,
        };

        NativeRenderPass = SDL3.SDL_BeginGPURenderPass(commandBuffer.NativeCommandBuffer, &info, 1, null);
    }

    public void Dispose()
    {
        SDL3.SDL_EndGPURenderPass(NativeRenderPass);
    }
}
