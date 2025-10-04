using System.Runtime.CompilerServices;
using Ignis.Platform.Sdl;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuSampler : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUSampler, SdlGpuException> NativeSampler { get; }

    public SdlGpuDevice Device { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuSampler(
        SdlGpuDevice device,
        SDL_GPUFilter minFilter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST,
        SDL_GPUFilter magFilter = SDL_GPUFilter.SDL_GPU_FILTER_NEAREST,
        SDL_GPUSamplerMipmapMode mipmapMode = SDL_GPUSamplerMipmapMode.SDL_GPU_SAMPLERMIPMAPMODE_NEAREST,
        SDL_GPUSamplerAddressMode addressModeU = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT,
        SDL_GPUSamplerAddressMode addressModeV = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT,
        SDL_GPUSamplerAddressMode addressModeW = SDL_GPUSamplerAddressMode.SDL_GPU_SAMPLERADDRESSMODE_REPEAT,
        float mipLodBias = 0.0f,
        float maxAnisotropy = 1.0f,
        SDL_GPUCompareOp compareOp = SDL_GPUCompareOp.SDL_GPU_COMPAREOP_ALWAYS,
        float minLod = float.MinValue,
        float maxLod = float.MaxValue,
        bool enableAnisotropy = false,
        bool enableComparison = false
    )
    {
        IgnisDebug.Assert(maxAnisotropy >= 1.0f, "Max anisotropy must be at least 1.0f.");
        IgnisDebug.Assert(minLod <= maxLod, "Min LOD must be less than or equal to Max LOD.");

        var createInfo = new SDL_GPUSamplerCreateInfo
        {
            min_filter = minFilter,
            mag_filter = magFilter,
            mipmap_mode = mipmapMode,
            address_mode_u = addressModeU,
            address_mode_v = addressModeV,
            address_mode_w = addressModeW,
            mip_lod_bias = mipLodBias,
            max_anisotropy = maxAnisotropy,
            compare_op = compareOp,
            min_lod = minLod,
            max_lod = maxLod,
            enable_anisotropy = enableAnisotropy,
            enable_compare = enableComparison,
            padding1 = 0,
            padding2 = 0,
            props = 0,
        };

        Device = device;
        NativeSampler = SDL3.SDL_CreateGPUSampler(device, &createInfo);
    }

    public void Dispose()
    {
        SDL3.SDL_ReleaseGPUSampler(Device, NativeSampler);
        NativeSampler.Dispose();
    }
}
