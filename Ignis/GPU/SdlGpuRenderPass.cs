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

    public void BindGraphicsPipeline(SdlGpuGraphicsPipeline pipeline)
    {
        SDL3.SDL_BindGPUGraphicsPipeline(NativeRenderPass, pipeline.NativeGraphicsPipeline);
    }

    public void BindVertexBuffer(SdlGpuBuffer vertexBuffer, uint firstSlot = 0, uint offset = 0)
    {
        var region = new SDL_GPUBufferBinding { buffer = vertexBuffer.NativeBuffer, offset = offset };
        SDL3.SDL_BindGPUVertexBuffers(NativeRenderPass, firstSlot, &region, 1);
    }

    public void BindIndexBuffer(
        SdlGpuBuffer indexBuffer,
        uint offset = 0,
        SDL_GPUIndexElementSize elementSize = SDL_GPUIndexElementSize.SDL_GPU_INDEXELEMENTSIZE_32BIT
    )
    {
        var region = new SDL_GPUBufferBinding { buffer = indexBuffer.NativeBuffer, offset = offset };
        SDL3.SDL_BindGPUIndexBuffer(NativeRenderPass, &region, elementSize);
    }

    public void BindFragmentSamplers(SdlGpuTexture texture, SdlGpuSampler sampler, uint index)
    {
        var binding = new SDL_GPUTextureSamplerBinding
        {
            texture = texture.NativeTexture,
            sampler = sampler.NativeSampler,
        };
        SDL3.SDL_BindGPUFragmentSamplers(NativeRenderPass, index, &binding, 1);
    }

    public void DrawPrimitives(uint vertexCount, uint firstVertex)
    {
        SDL3.SDL_DrawGPUPrimitives(NativeRenderPass, vertexCount, 1, firstVertex, 0);
    }

    public void DrawIndexedPrimitives(uint numIndices, uint startIndex, int vertexOffset)
    {
        SDL3.SDL_DrawGPUIndexedPrimitives(NativeRenderPass, numIndices, 1, startIndex, vertexOffset, 0);
    }

    public void Dispose()
    {
        SDL3.SDL_EndGPURenderPass(NativeRenderPass);
    }
}
