using Ignis.Platform;
using SDL;

namespace Ignis.GPU;

internal unsafe class SdlGpuDevice : IDisposable
{
    public SdlPointer<SDL_GPUDevice, SdlGpuException> NativeDevice { get; }

    public SDL_GPUShaderFormat ShaderLanguage { get; }

    public SdlGpuDevice(Window window, bool debugMode = false)
    {
        NativeDevice = SDL3.SDL_CreateGPUDevice(
            SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV
                | SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL
                | SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL,
            debugMode,
            (byte*)null
        );

        ShaderLanguage = GetChosenShaderFormat();
    }

    public void Dispose()
    {
        SDL3.SDL_DestroyGPUDevice(NativeDevice);
    }

    public static implicit operator SDL_GPUDevice*(SdlGpuDevice sdlDevice) => sdlDevice.NativeDevice;

    private SDL_GPUShaderFormat GetChosenShaderFormat()
    {
        var formats = SDL3.SDL_GetGPUShaderFormats(this);

        if (formats.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_SPIRV;

        if (formats.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXBC))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXBC;

        if (formats.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_DXIL;

        if (formats.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_MSL;

        if (formats.HasFlag(SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_METALLIB))
            return SDL_GPUShaderFormat.SDL_GPU_SHADERFORMAT_METALLIB;

        throw new SdlGpuException("No supported shader formats found for device");
    }
}
