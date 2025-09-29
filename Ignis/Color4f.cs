using System.Diagnostics;
using System.Numerics;

namespace Ignis;

/// <summary>
/// Color based on 4 floating point values (typically 0.0f - 1.0f)
/// This type is generally used for interaction with the GPU such as vertex colors.
/// </summary>
[DebuggerDisplay("R: {R} G: {G} B: {B} A: {A}")]
public struct Color4f : IEquatable<Color4f>
{
    public static readonly Color4f Transparent = new(0, 0, 0, 0);
    public static readonly Color4f Black = new(0, 0, 0, 255);
    public static readonly Color4f White = new(255, 255, 255, 255);
    public static readonly Color4f Red = new(255, 0, 0, 255);
    public static readonly Color4f Green = new(0, 255, 0, 255);
    public static readonly Color4f Blue = new(0, 0, 255, 255);
    public static readonly Color4f Yellow = new(255, 255, 0, 255);
    public static readonly Color4f Magenta = new(255, 0, 255, 255);
    public static readonly Color4f Cyan = new(0, 255, 255, 255);
    public static readonly Color4f Orange = new(255, 127, 0, 255);
    public static readonly Color4f Gray = new(127, 127, 127, 255);
    public static readonly Color4f LightGray = new(191, 191, 191, 255);
    public static readonly Color4f DarkGray = new(63, 63, 63, 255);
    public static readonly Color4f DarkGreen = new(0, 127, 0, 255);
    public static readonly Color4f DarkRed = new(127, 0, 0, 255);
    public static readonly Color4f DarkBlue = new(0, 0, 127, 255);
    public static readonly Color4f DarkYellow = new(127, 127, 0, 255);
    public static readonly Color4f DarkMagenta = new(127, 0, 127, 255);
    public static readonly Color4f DarkCyan = new(0, 127, 127, 255);
    public static readonly Color4f DarkOrange = new(127, 63, 0, 255);
    public static readonly Color4f LightGreen = new(127, 255, 127, 255);
    public static readonly Color4f LightRed = new(255, 127, 127, 255);
    public static readonly Color4f LightBlue = new(127, 127, 255, 255);
    public static readonly Color4f LightYellow = new(255, 255, 127, 255);
    public static readonly Color4f LightMagenta = new(255, 127, 255, 255);
    public static readonly Color4f LightCyan = new(127, 255, 255, 255);
    public static readonly Color4f LightOrange = new(255, 191, 127, 255);

    /// <summary>
    /// Red
    /// </summary>
    public float R { get; set; }

    /// <summary>
    /// Green
    /// </summary>
    public float G { get; set; }

    /// <summary>
    /// Blue
    /// </summary>
    public float B { get; set; }

    /// <summary>
    /// Alpha
    /// </summary>
    public float A { get; set; }

    /// <summary>
    /// Create a color with integer values (0 - 255)
    /// </summary>
    /// <param name="r">Red (0 - 255)</param>
    /// <param name="g">Green (0 - 255)</param>
    /// <param name="b">Blue (0 - 255)</param>
    /// <param name="a">Alpha (0 - 255)</param>
    public Color4f(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        const float fByteMaxValue = byte.MaxValue;
        R = r / fByteMaxValue;
        G = g / fByteMaxValue;
        B = b / fByteMaxValue;
        A = a / fByteMaxValue;
    }

    /// <summary>
    /// Create a new color from the given one with a different alpha
    /// </summary>
    /// <param name="color">The base color</param>
    /// <param name="a">The new alpha value (0.0f - 1.0f)</param>
    public Color4f(Color4f color, float a)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = a;
    }

    /// <summary>
    /// Create a new color from the given one with a different alpha
    /// </summary>
    /// <param name="color">The base color</param>
    /// <param name="a">The new alpha value (0 - 255)</param>
    public Color4f(Color4f color, byte a)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = a / (float)byte.MaxValue;
    }

    /// <summary>
    /// Create a color with floating point values (0.0f - 1.0f)
    /// </summary>
    /// <param name="r">Red (0.0f - 1.0f)</param>
    /// <param name="g">Green (0.0f - 1.0f)</param>
    /// <param name="b">Blue (0.0f - 1.0f)</param>
    /// <param name="a">Alpha (0.0f - 1.0f)</param>
    public Color4f(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    /// <summary>
    /// Create a color with floating point values (0.0f - 1.0)
    /// </summary>
    /// <param name="r">Red (0.0 - 1.0)</param>
    /// <param name="g">Green (0.0 - 1.0)</param>
    /// <param name="b">Blue (0.0 - 1.0)</param>
    /// <param name="a">Alpha (0.0 - 1.0)</param>
    public Color4f(double r, double g, double b, double a = 1.0)
        : this((float)r, (float)g, (float)b, (float)a) { }

    /// <summary>
    /// Create a color with floating point values (0.0f - 1.0f) from a Vector3
    /// </summary>
    /// <param name="color">A vector containing a color, where X is Red, Y is Green and Z is blue.</param>
    public Color4f(Vector3 color)
        : this(color.X, color.Y, color.Z) { }

    /// <summary>
    /// Create a color with floating point values (0.0f - 1.0f) from a Vector4
    /// </summary>
    /// <param name="color">A vector containing a color, where X is Red, Y is Green, Z is blue and W is alpha.</param>
    public Color4f(Vector4 color)
        : this(color.X, color.Y, color.Z, color.W) { }

    /// <summary>
    /// Create a new color from a given Color4b
    /// </summary>
    /// <param name="color">A Color4b value</param>
    public Color4f(Color4b color)
        : this(color.R, color.G, color.B, color.A) { }

    /// <summary>
    /// Convert the color to a Vector3; where X = R, Y = G and Z = B
    /// </summary>
    public readonly Vector3 ToVector3()
    {
        return new Vector3(R, G, B);
    }

    /// <summary>
    /// Convert the color to a Vector4; where X = R, Y = G, Z = B and W = A
    /// </summary>
    public readonly Vector4 ToVector4()
    {
        return new Vector4(R, G, B, A);
    }

    public static bool operator ==(Color4f a, Color4f b)
    {
        return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
    }

    public static bool operator !=(Color4f a, Color4f b)
    {
        return !(a == b);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    public readonly bool Equals(Color4f other)
    {
        return this == other;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Color4f color && Equals(color);
    }
}
