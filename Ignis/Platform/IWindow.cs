using Ignis.Math;

namespace Ignis.Platform;

public interface IWindow : IDisposable
{
    public string Title { get; set; }

    /// <summary>
    /// The dimensions of the window
    /// </summary>
    public Vector2i Dimensions { get; }

    /// <summary>
    /// Returns true when the user has tried to close the window; usually by clicking the X.
    /// </summary>
    public bool Closing { get; }

    /// <summary>
    /// Attempt to close the window. Depending on the platform, this may not be possible.
    /// </summary>
    public void Close();
}
