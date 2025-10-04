using System.Runtime.CompilerServices;
using Ignis.Platform;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuGraphicsPipeline : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUGraphicsPipeline, SdlGpuException> NativeGraphicsPipeline { get; }

    public SdlGpuDevice Device { get; }

    private readonly ReferenceCounted<SdlGpuShader> _vertexShaderRef;
    public SdlGpuShader VertexShaderRef => _vertexShaderRef;

    private readonly ReferenceCounted<SdlGpuShader> _fragmentShaderRef;
    public SdlGpuShader FragmentShaderRef => _fragmentShaderRef;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuGraphicsPipeline(
        SdlGpuDevice device,
        SdlGpuShader vertexShaderRef,
        SdlGpuShader fragmentShaderRef,
        SDL_GPUVertexInputState vertexInputState,
        SDL_GPUPrimitiveType primitiveType,
        SDL_GPURasterizerState rasterizerState,
        SDL_GPUMultisampleState multisampleState,
        SDL_GPUDepthStencilState depthStencilState,
        SDL_GPUGraphicsPipelineTargetInfo targetInfo
    )
    {
        Device = device;
        _vertexShaderRef = vertexShaderRef;
        _fragmentShaderRef = fragmentShaderRef;

        var info = new SDL_GPUGraphicsPipelineCreateInfo
        {
            vertex_shader = vertexShaderRef.NativeShader,
            fragment_shader = fragmentShaderRef.NativeShader,
            vertex_input_state = vertexInputState,
            primitive_type = primitiveType,
            rasterizer_state = rasterizerState,
            multisample_state = multisampleState,
            depth_stencil_state = depthStencilState,
            target_info = targetInfo,
            props = 0,
        };

        NativeGraphicsPipeline = SDL3.SDL_CreateGPUGraphicsPipeline(device, &info);
    }

    public void Dispose()
    {
        SDL3.SDL_ReleaseGPUGraphicsPipeline(Device, NativeGraphicsPipeline);
        NativeGraphicsPipeline.Dispose();

        _vertexShaderRef.Dispose();
        _fragmentShaderRef.Dispose();
    }
}
