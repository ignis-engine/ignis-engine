using Ignis.Interop;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuShader : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUShader, SdlGpuException> NativeShader { get; }

    public SdlGpuDevice Device { get; }

    public SDL_GPUShaderStage Stage { get; }

    public uint NumSamplers { get; }

    public uint NumStorageTextures { get; }

    public uint NumStorageBuffers { get; }

    public uint NumUniformBuffers { get; }

    public SdlGpuShader(
        SdlGpuDevice device,
        byte[] shaderBlob,
        string entryPoint,
        SDL_GPUShaderStage stage,
        uint numSamplers,
        uint numStorageTextures,
        uint numStorageBuffers,
        uint numUniformBuffers
    )
    {
        Device = device;
        Stage = stage;
        NumSamplers = numSamplers;
        NumStorageTextures = numStorageTextures;
        NumStorageBuffers = numStorageBuffers;
        NumUniformBuffers = numUniformBuffers;

        using var entryPointBytes = Utf8Interop.Utf8EncodeHeap(entryPoint);

        fixed (byte* codePtr = shaderBlob)
        {
            var info = new SDL_GPUShaderCreateInfo
            {
                code_size = (nuint)shaderBlob.Length,
                code = codePtr,
                entrypoint = entryPointBytes,
                format = device.ShaderLanguage,
                stage = stage,
                num_samplers = numSamplers,
                num_storage_textures = numStorageTextures,
                num_storage_buffers = numStorageBuffers,
                num_uniform_buffers = numUniformBuffers,
                props = 0,
            };

            NativeShader = SDL3.SDL_CreateGPUShader(device, &info);
        }
    }

    public void Dispose()
    {
        SDL3.SDL_ReleaseGPUShader(Device, NativeShader);
        NativeShader.Dispose();
    }
}
