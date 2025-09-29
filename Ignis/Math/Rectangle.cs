using System.Diagnostics;
using System.Numerics;

namespace Ignis.Math;

/// <summary>
/// Represents a generic rectangle with coordinates defined by a type parameter.
/// </summary>
/// <typeparam name="T">The type parameter that defines the coordinate values.</typeparam>
[DebuggerDisplay("(Left: {Left} Top: {Top} Right: {Right} Bottom: {Bottom})")]
public sealed class Rectangle<T> : IEquatable<Rectangle<T>>, IEqualityOperators<Rectangle<T>, Rectangle<T>, bool>
    where T : INumber<T>
{
    /// <summary>
    /// Create an empty rectangle. This initializes all values to 0.
    /// </summary>
    public Rectangle()
    {
        Left = T.Zero;
        Top = T.Zero;
        Right = T.Zero;
        Bottom = T.Zero;
    }

    /// <summary>
    /// Create a rectangle based on the given values.
    /// </summary>
    public Rectangle(T left, T top, T right, T bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    /// <summary>
    /// Create a rectangle based on the given values.
    /// </summary>
    public Rectangle(Vector2 leftTop, Vector2 rightBottom)
    {
        Left = T.CreateTruncating(leftTop.X);
        Top = T.CreateTruncating(leftTop.Y);
        Right = T.CreateTruncating(rightBottom.X);
        Bottom = T.CreateTruncating(rightBottom.Y);
    }

    /// <summary>
    /// Create a rectangle with the given size at position 0, 0
    /// </summary>
    public Rectangle(Vector2 size)
    {
        Left = T.Zero;
        Top = T.Zero;
        Right = T.CreateTruncating(size.X);
        Bottom = T.CreateTruncating(size.Y);
    }

    /// <summary>
    /// Create a rectangle based on the given values. Performs truncating conversion.
    /// </summary>
    /// <param name="other">Another rectangle</param>
    public Rectangle(Rectangle<float> other)
    {
        Left = T.CreateTruncating(other.Left);
        Top = T.CreateTruncating(other.Top);
        Right = T.CreateTruncating(other.Right);
        Bottom = T.CreateTruncating(other.Bottom);
    }

    /// <summary>
    /// Create a rectangle based on the given values. Performs truncating conversion.
    /// </summary>
    /// <param name="other">Another rectangle</param>
    public Rectangle(Rectangle<int> other)
    {
        Left = T.CreateTruncating(other.Left);
        Top = T.CreateTruncating(other.Top);
        Right = T.CreateTruncating(other.Right);
        Bottom = T.CreateTruncating(other.Bottom);
    }

    /// <summary>
    /// Gets or sets the left coordinate of the rectangle.
    /// </summary>
    public T Left { get; set; }

    /// <summary>
    /// Gets or sets the top coordinate of the rectangle.
    /// </summary>
    public T Top { get; set; }

    /// <summary>
    /// Gets or sets the right coordinate of the rectangle.
    /// </summary>
    public T Right { get; set; }

    /// <summary>
    /// Gets or sets the bottom coordinate of the rectangle.
    /// </summary>
    public T Bottom { get; set; }

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    public T Width => Right - Left;

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    public T Height => Bottom - Top;

    /// <summary>
    /// Gets the top-left position of the rectangle as a 2D vector.
    /// </summary>
    public Vector2 LeftTop => new(Convert.ToSingle(Left), Convert.ToSingle(Top));

    /// <summary>
    /// Gets the left-bottom position of the rectangle as a 2D vector.
    /// </summary>
    public Vector2 LeftBottom => new(Convert.ToSingle(Left), Convert.ToSingle(Bottom));

    /// <summary>
    /// Gets the right-top position of the rectangle as a 2D vector.
    /// </summary>
    public Vector2 RightTop => new(Convert.ToSingle(Right), Convert.ToSingle(Top));

    /// <summary>
    /// Gets the bottom-right position of the rectangle as a 2D vector.
    /// </summary>
    public Vector2 RightBottom => new(Convert.ToSingle(Right), Convert.ToSingle(Bottom));

    /// <summary>
    /// Gets the dimensions (width and height) of the rectangle as a 2D vector.
    /// </summary>
    public Vector2 Dimensions => new(Convert.ToSingle(Width), Convert.ToSingle(Height));

    /// <summary>
    /// Gets a value indicating whether the rectangle is empty (has no area).
    /// </summary>
    public bool Empty => Left == Right || Top == Bottom;

    /// <summary>
    /// Gets a value indicating whether the rectangle is null (all sides are equal).
    /// </summary>
    public bool Null => Left == Right && Top == Bottom;

    /// <summary>
    /// Gets a value indicating whether the rectangle is valid (left &lt; right and top &lt; bottom).
    /// You can use <see cref="Normalize"/> or <see cref="Normalized"/> to normalize the rectangle to make it valid.
    /// </summary>
    public bool Valid => Left < Right && Top < Bottom;

    /// <summary>
    /// Normalize the rectangle so that left is less than right and top is less than bottom.
    /// The rectangle will always be <see cref="Valid"/> after calling this if the rectangle is not <see cref="Null"/>.
    /// </summary>
    public void Normalize()
    {
        if (Left > Right)
            (Left, Right) = (Right, Left);

        if (Top > Bottom)
            (Top, Bottom) = (Bottom, Top);
    }

    /// <summary>
    /// Return a new rectangle with the same values as this rectangle, but normalized.
    /// The returned rectangle will always be <see cref="Valid"/> after calling this
    /// if the current rectangle is not <see cref="Null"/>.
    /// </summary>
    /// <returns>A normalized rectangle</returns>
    public Rectangle<T> Normalized()
    {
        var rectangle = new Rectangle<T>(Left, Top, Right, Bottom);
        rectangle.Normalize();
        return rectangle;
    }

    /// <summary>
    /// Create a rectangle within the given dimensions scaled to the given rectangle.
    /// The given rectangle must contain values between 0 and 1.
    /// This is useful for creating rectangles that are relative to the size of a window.
    /// </summary>
    /// <param name="dimensions">The dimensions of a rectangle (for example a window)</param>
    /// <param name="rectangle">A scaling rectangle</param>
    /// <returns>A scaled rectangle</returns>
    public static Rectangle<T> CreateScaled(Vector2 dimensions, Rectangle<T> rectangle)
    {
        return new(
            T.CreateTruncating(float.CreateTruncating(rectangle.Left) * dimensions.X),
            T.CreateTruncating(float.CreateTruncating(rectangle.Top) * dimensions.Y),
            T.CreateTruncating(float.CreateTruncating(rectangle.Right) * dimensions.X),
            T.CreateTruncating(float.CreateTruncating(rectangle.Bottom) * dimensions.Y)
        );
    }

    static bool IEqualityOperators<Rectangle<T>, Rectangle<T>, bool>.operator ==(
        Rectangle<T>? left,
        Rectangle<T>? right
    )
    {
        if (left is null || right is null)
            return false;

        return left.Left == right.Left
            && left.Top == right.Top
            && left.Right == right.Right
            && left.Bottom == right.Bottom;
    }

    static bool IEqualityOperators<Rectangle<T>, Rectangle<T>, bool>.operator !=(
        Rectangle<T>? left,
        Rectangle<T>? right
    )
    {
        if (left is null || right is null)
            return true;

        return left.Left != right.Left
            || left.Top != right.Top
            || left.Right != right.Right
            || left.Bottom != right.Bottom;
    }

    public bool Equals(Rectangle<T>? other)
    {
        return other is not null
            && Left == other.Left
            && Top == other.Top
            && Right == other.Right
            && Bottom == other.Bottom;
    }

    /// <summary>
    /// Sets the position of the rectangle based on the specified 2D position vector.
    /// </summary>
    /// <param name="position">The 2D position vector to set the rectangle's position to.</param>
    public void SetPosition(Vector2 position)
    {
        Right = T.CreateTruncating(position.X) + Width;
        Bottom = T.CreateTruncating(position.Y) + Height;
    }

    /// <summary>
    /// Sets the position of the rectangle based on the specified coordinates.
    /// </summary>
    public void SetPosition(T x, T y)
    {
        Right = x + Width;
        Bottom = y + Height;
    }

    /// <summary>
    /// Translate (move) the rectangle by the specified 2D translation vector.
    /// </summary>
    public void Translate(Vector2 translation)
    {
        Left += T.CreateTruncating(translation.X);
        Top += T.CreateTruncating(translation.Y);
        Right += T.CreateTruncating(translation.X);
        Bottom += T.CreateTruncating(translation.Y);
    }

    /// <summary>
    /// Translate (move) the rectangle by the specified coordinates.
    /// </summary>
    public void Translate(T x, T y)
    {
        Left += x;
        Top += y;
        Right += x;
        Bottom += y;
    }

    /// <summary>
    /// Sets the size of the rectangle based on the specified 2D size vector.
    /// </summary>
    public void SetSize(Vector2 size)
    {
        Right = T.CreateTruncating(size.X) + Left;
        Bottom = T.CreateTruncating(size.Y) + Top;
    }

    /// <summary>
    /// Sets the size of the rectangle based on the specified width and height.
    /// </summary>
    public void SetSize(T width, T height)
    {
        Right = width + Left;
        Bottom = height + Top;
    }

    /// <summary>
    /// Scale all values of this rectangle by the given value.
    /// </summary>
    public void Scale(T value)
    {
        Left *= value;
        Top *= value;
        Right *= value;
        Bottom *= value;
    }

    /// <summary>
    /// Return a new rectangle with all values of this rectangle scaled by the given value.
    /// </summary>
    public Rectangle<T> Scaled(T value)
    {
        return new(Left * value, Top * value, Right * value, Bottom * value);
    }

    /// <summary>
    /// Check if a given point is inside this rectangle.
    /// </summary>
    /// <param name="x">The X coordinate</param>
    /// <param name="y">The Y coordinate</param>
    /// <returns>True if a given point is inside this rectangle.</returns>
    public bool Contains(T x, T y)
    {
        return x >= Left && x <= Right && y >= Top && y <= Bottom;
    }

    /// <summary>
    /// Check if a given point is inside this rectangle.
    /// </summary>
    /// <param name="point">The coordinates</param>
    /// <returns>True if a given point is inside this rectangle.</returns>
    public bool Contains(Vector2 point)
    {
        return Contains(T.CreateTruncating(point.X), T.CreateTruncating(point.Y));
    }

    /// <summary>
    /// Check if a given rectangle overlaps this rectangle.
    /// Both rectangles must be <see cref="Valid"/>, otherwise the result is undefined.
    /// </summary>
    /// <param name="other">Another rectangle</param>
    /// <returns>True if the given rectangle overlaps; otherwise false.</returns>
    public bool Overlaps(Rectangle<T> other)
    {
        IgnisDebug.Assert(Valid);
        IgnisDebug.Assert(other.Valid);

        return Left < other.Right && Right > other.Left && Top < other.Bottom && Bottom > other.Top;
    }

    /// <summary>
    /// Combine this rectangle with another rectangle to create a new rectangle that contains both.
    /// Assumes that the given rectangles are normalized.
    /// </summary>
    public Rectangle<T> Union(Rectangle<T> other)
    {
        return new(
            T.Min(Left, other.Left),
            T.Min(Top, other.Top),
            T.Max(Right, other.Right),
            T.Max(Bottom, other.Bottom)
        );
    }

    /// <summary>
    /// Combine this rectangle with multiple other rectangles to create a new rectangle that contains all.
    /// Assumes that the given rectangles are normalized.
    /// </summary>
    public Rectangle<T> Union(params Rectangle<T>[] other)
    {
        var result = new Rectangle<T>(Left, Top, Right, Bottom);

        foreach (var rectangle in other)
        {
            result = result.Union(rectangle);
        }

        return result;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public override bool Equals(object? obj)
    {
        return obj is Rectangle<T> color && Equals(color);
    }

    public override string ToString()
    {
        return $"({Left}, {Top}, {Right}, {Bottom})";
    }
}
