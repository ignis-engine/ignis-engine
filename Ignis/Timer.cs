using SDL;

namespace Ignis;

/// <summary>
/// Simple high resolution timer class to calculate delta time
/// </summary>
public class Timer
{
    private readonly float _frequency = SDL3.SDL_GetPerformanceFrequency();
    private ulong _previous = SDL3.SDL_GetPerformanceCounter();

    /// <summary>
    /// Get the delta time since the last call to this function.
    /// </summary>
    public float GetDeltaTime()
    {
        var now = SDL3.SDL_GetPerformanceCounter();
        var deltaTime = (now - _previous) / _frequency;
        _previous = now;
        return deltaTime;
    }
}
