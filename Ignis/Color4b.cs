using System.Diagnostics;

namespace Ignis;

[DebuggerDisplay("R: {R} G: {G} B: {B} A: {A}")]
public struct Color4b : IEquatable<Color4b>
{
    public static readonly Color4b Transparent = new(0, 0, 0, 0);
    public static readonly Color4b Black = new(0, 0, 0, 255);
    public static readonly Color4b White = new(255, 255, 255, 255);
    public static readonly Color4b Red = new(255, 0, 0, 255);
    public static readonly Color4b Green = new(0, 255, 0, 255);
    public static readonly Color4b Blue = new(0, 0, 255, 255);
    public static readonly Color4b Yellow = new(255, 255, 0, 255);
    public static readonly Color4b Magenta = new(255, 0, 255, 255);
    public static readonly Color4b Cyan = new(0, 255, 255, 255);
    public static readonly Color4b Orange = new(255, 127, 0, 255);
    public static readonly Color4b Gray = new(127, 127, 127, 255);
    public static readonly Color4b LightGray = new(191, 191, 191, 255);
    public static readonly Color4b DarkGray = new(63, 63, 63, 255);
    public static readonly Color4b DarkGreen = new(0, 127, 0, 255);
    public static readonly Color4b DarkRed = new(127, 0, 0, 255);
    public static readonly Color4b DarkBlue = new(0, 0, 127, 255);
    public static readonly Color4b DarkYellow = new(127, 127, 0, 255);
    public static readonly Color4b DarkMagenta = new(127, 0, 127, 255);
    public static readonly Color4b DarkCyan = new(0, 127, 127, 255);
    public static readonly Color4b DarkOrange = new(127, 63, 0, 255);
    public static readonly Color4b LightGreen = new(127, 255, 127, 255);
    public static readonly Color4b LightRed = new(255, 127, 127, 255);
    public static readonly Color4b LightBlue = new(127, 127, 255, 255);
    public static readonly Color4b LightYellow = new(255, 255, 127, 255);
    public static readonly Color4b LightMagenta = new(255, 127, 255, 255);
    public static readonly Color4b LightCyan = new(127, 255, 255, 255);
    public static readonly Color4b LightOrange = new(255, 191, 127, 255);

    public uint Value = 0xffffffff;

    /// <summary>
    /// Red
    /// </summary>
    public readonly byte R => (byte)(Value);

    /// <summary>
    /// Green
    /// </summary>
    public readonly byte G => (byte)(Value >> 8);

    /// <summary>
    /// Blue
    /// </summary>
    public readonly byte B => (byte)(Value >> 16);

    /// <summary>
    /// Alpha
    /// </summary>
    public readonly byte A => (byte)(Value >> 24);

    /// <summary>
    /// Returns true if the color is grayscale (Red, Green and Blue are equal)
    /// </summary>
    public readonly bool IsGrayscale => R == G && G == B;

    /// <summary>
    /// Create a color from a raw 32-bit unsigned integer value
    /// </summary>
    /// <param name="value"></param>
    public Color4b(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// Create a color with integer values (0 - 255)
    /// </summary>
    /// <param name="r">Red (0 - 255)</param>
    /// <param name="g">Green (0 - 255)</param>
    /// <param name="b">Blue (0 - 255)</param>
    /// <param name="a">Alpha (0 - 255</param>
    public Color4b(byte r, byte g, byte b, byte a = byte.MaxValue)
        : this((uint)(((uint)a) << 24 | ((uint)b) << 16 | ((uint)g) << 8 | (uint)r)) { }

    /// <summary>
    /// Create a new color from the given one with a different alpha
    /// </summary>
    /// <param name="color">The base color</param>
    /// <param name="a">The new alpha value (0 - 255)</param>
    public Color4b(Color4b color, byte a)
        : this(color.R, color.G, color.B, a) { }

    /// <summary>
    /// Create a new color from a given Color4f
    /// </summary>
    /// <param name="color">A Color4f value</param>
    public Color4b(Color4f color)
        : this(
            (byte)(color.R * byte.MaxValue),
            (byte)(color.G * byte.MaxValue),
            (byte)(color.B * byte.MaxValue),
            (byte)(color.A * byte.MaxValue)
        ) { }

    public static bool operator ==(Color4b a, Color4b b)
    {
        return a.Value == b.Value;
    }

    public static bool operator !=(Color4b a, Color4b b)
    {
        return !(a == b);
    }

    public override readonly unsafe int GetHashCode()
    {
        fixed (void* ptr = &Value)
        {
            return *(int*)ptr;
        }
    }

    public readonly bool Equals(Color4b other)
    {
        return this == other;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Color4f color && Equals(color);
    }

    public override readonly string ToString()
    {
        return $"R: {R} G: {G} B: {B} A: {A} (#{R:X2}{G:X2}{B:X2}{A:X2})";
    }
}
