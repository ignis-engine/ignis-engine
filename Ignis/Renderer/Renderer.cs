using Ignis.GPU;
using Ignis.Platform;
using SDL;

namespace Ignis.Renderer;

public unsafe class Renderer : IDisposable
{
    private Window _window;
    private SdlGpuDevice _device;
    private SdlGpuShader _vertexShader;
    private SdlGpuShader _fragmentShader;
    private SdlGpuGraphicsPipeline _pipeline;

    public Renderer()
    {
        _window = new Window("Ignis Game Engine", new Math.Vector2i(1920, 1080));
        _device = new SdlGpuDevice(_window);

        var vertexBytes = File.ReadAllBytes("RawTriangle.vert.spv");
        _vertexShader = new SdlGpuShader(
            _device,
            vertexBytes,
            "main",
            SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_VERTEX,
            0,
            0,
            0,
            0
        );

        var fragmentBytes = File.ReadAllBytes("SolidColor.frag.spv");
        _fragmentShader = new SdlGpuShader(
            _device,
            fragmentBytes,
            "main",
            SDL_GPUShaderStage.SDL_GPU_SHADERSTAGE_FRAGMENT,
            0,
            0,
            0,
            0
        );

        SDL_GPUVertexInputState inputState = new();
        SDL_GPUPrimitiveType primitiveType = SDL_GPUPrimitiveType.SDL_GPU_PRIMITIVETYPE_TRIANGLELIST;
        SDL_GPURasterizerState rasterizerState = new();
        SDL_GPUMultisampleState multisampleState = new();
        SDL_GPUDepthStencilState depthStencilState = new();

        SDL_GPUColorTargetDescription colorTargetDesc = new() { format = _device.GetSwapchainTextureFormat() };
        SDL_GPUGraphicsPipelineTargetInfo targetInfo = new()
        {
            num_color_targets = 1,
            color_target_descriptions = &colorTargetDesc,
        };

        _pipeline = new SdlGpuGraphicsPipeline(
            _device,
            _vertexShader,
            _fragmentShader,
            inputState,
            primitiveType,
            rasterizerState,
            multisampleState,
            depthStencilState,
            targetInfo
        );
    }

    public void Test()
    {
        while (!_window.Closing)
        {
            SDL_Event evt;
            while (SDL3.SDL_PollEvent(&evt))
            {
                if (evt.Type == SDL_EventType.SDL_EVENT_QUIT)
                    _window.Close();
            }

            using var cmd = new SdlGpuCommandBuffer(_device);
            var swapchainTexture = cmd.WaitAndAcquireSwapchainTexture(_window);
            using var renderPass = new SdlGpuRenderPass(
                cmd,
                swapchainTexture,
                new SDL_FColor
                {
                    r = 1,
                    g = 0,
                    b = 1,
                    a = 1,
                }
            );
            renderPass.BindGraphicsPipeline(_pipeline);

            var windowDimensions = _window.Dimensions;
            var viewport = new SDL_GPUViewport
            {
                x = 0,
                y = 0,
                w = windowDimensions.X,
                h = windowDimensions.Y,
                min_depth = 0,
                max_depth = 100.0f,
            };
            renderPass.SetViewport(viewport);
            renderPass.DrawPrimitives(3, 0);
        }
    }

    public void Dispose() { }
}
