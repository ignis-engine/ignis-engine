using System.Runtime.CompilerServices;
using Ignis.Platform;
using SDL;

namespace Ignis.GPU;

[Flags]
internal enum GpuTextureUsageFlags : uint
{
    Sampler = SDL3.SDL_GPU_TEXTUREUSAGE_SAMPLER,
    ColorTarget = SDL3.SDL_GPU_TEXTUREUSAGE_COLOR_TARGET,
    DepthStencilTarget = SDL3.SDL_GPU_TEXTUREUSAGE_DEPTH_STENCIL_TARGET,
    GraphicsStorageRead = SDL3.SDL_GPU_TEXTUREUSAGE_GRAPHICS_STORAGE_READ,
    ComputeStorageRead = SDL3.SDL_GPU_TEXTUREUSAGE_COMPUTE_STORAGE_READ,
    ComputeStorageWrite = SDL3.SDL_GPU_TEXTUREUSAGE_COMPUTE_STORAGE_WRITE,
    ComputeStorageSimultaneousReadWrite = SDL3.SDL_GPU_TEXTUREUSAGE_COMPUTE_STORAGE_SIMULTANEOUS_READ_WRITE,
}

internal unsafe class SdlGpuTexture : ReferenceCountedBase, IDisposable
{
    public SdlPointer<SDL_GPUTexture, SdlGpuException> NativeTexture { get; }

    public SdlGpuDevice Device { get; }

    public uint Width { get; }

    public uint Height { get; }

    public uint LayerCountOrDepth { get; }

    public uint NumLevels { get; }

    public SDL_GPUTextureFormat Format { get; }

    public SDL_GPUTextureType Type { get; }

    public GpuTextureUsageFlags Usage { get; }

    public SDL_GPUSampleCount SampleCount { get; }

    public bool OwnsData { get; }

    public string DebugLabel
    {
        set => SDL3.SDL_SetGPUTextureName(Device, NativeTexture, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuTexture(
        SdlGpuDevice device,
        uint width,
        uint height,
        SDL_GPUTextureFormat format,
        SDL_GPUSampleCount sampleCount = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
        string? debugLabel = null
    )
        : this(
            device,
            width,
            height,
            1,
            1,
            format,
            SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D,
            GpuTextureUsageFlags.Sampler,
            sampleCount,
            debugLabel
        ) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuTexture(
        SdlGpuDevice device,
        uint width,
        uint height,
        uint layerCountOrDepth,
        uint numLevels,
        SDL_GPUTextureFormat format,
        SDL_GPUTextureType type,
        GpuTextureUsageFlags usage,
        SDL_GPUSampleCount sampleCount = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1,
        string? debugLabel = null
    )
    {
        Device = device;
        Width = width;
        Height = height;
        LayerCountOrDepth = layerCountOrDepth;
        NumLevels = numLevels;
        Format = format;
        Type = type;
        Usage = usage;
        SampleCount = sampleCount;

        var info = new SDL_GPUTextureCreateInfo
        {
            type = type,
            format = format,
            usage = (SDL_GPUTextureUsageFlags)usage,
            width = width,
            height = height,
            layer_count_or_depth = layerCountOrDepth,
            num_levels = numLevels,
            sample_count = sampleCount,
            props = 0,
        };

        NativeTexture = SDL3.SDL_CreateGPUTexture(device, &info);

        if (debugLabel is not null)
            DebugLabel = debugLabel;

        OwnsData = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SdlGpuTexture(
        SdlGpuDevice device,
        SDL_GPUTexture* texture,
        uint width,
        uint height,
        SDL_GPUTextureFormat format
    )
    {
        Device = device;
        Width = width;
        Height = height;
        LayerCountOrDepth = 1;
        NumLevels = 1;
        Format = format;
        Type = SDL_GPUTextureType.SDL_GPU_TEXTURETYPE_2D;
        Usage = 0;
        SampleCount = SDL_GPUSampleCount.SDL_GPU_SAMPLECOUNT_1;
        NativeTexture = texture;
        OwnsData = false;
    }

    public void Dispose()
    {
        if (OwnsData)
            SDL3.SDL_ReleaseGPUTexture(Device, NativeTexture);

        NativeTexture.Dispose();
    }
}
