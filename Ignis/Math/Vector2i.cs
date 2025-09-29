using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Ignis.Math;

[DebuggerDisplay("({X}, {Y})")]
public struct Vector2i : IEquatable<Vector2i>
{
    /// <summary>
    /// The X component of the vector.
    /// </summary>
    public int X;

    /// <summary>
    /// The Y component of the vector.
    /// </summary>
    public int Y;

    internal const int Count = 2;

    /// <summary>
    /// Create a vector where both X and Y are set to the given value.
    /// </summary>
    /// <param name="value"></param>
    public Vector2i(int value)
        : this(value, value) { }

    /// <summary>
    /// Create a vector with the given X and Y components.
    /// </summary>
    public Vector2i(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Create a Vector2i from an existing float based Vector2.
    /// </summary>
    public Vector2i(Vector2 vector)
    {
        X = (int)vector.X;
        Y = (int)vector.Y;
    }

    /// <summary>
    /// Returns a vector whose 2 elements are equal to zero.
    /// </summary>
    public static Vector2i Zero => new(0, 0);

    /// <summary>
    /// Returns a vector whose 2 elements are equal to one.
    /// </summary>
    public static Vector2i One => new(1, 1);

    /// <summary>
    /// Gets the vector (1,0).
    /// </summary>
    public static Vector2i UnitX => new(1, 0);

    /// <summary>
    /// Gets the vector (0,1).
    /// </summary>
    public static Vector2i UnitY => new(0, 1);

    /// <summary>
    /// Get the area of the vector (if the vector represents a width/height).
    /// Returns X * Y.
    /// </summary>
    public readonly int Area => X * Y;

    public int this[int index]
    {
        get => GetElement(this, index);
        set => this = WithElement(this, index, value);
    }

    /// <summary>Gets the element at the specified index.</summary>
    /// <param name="vector">The vector of the element to get.</param>
    /// <param name="index">The index of the element to get.</param>
    /// <returns>The value of the element at <paramref name="index" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    internal static int GetElement(Vector2i vector, int index)
    {
        if ((uint)index >= Count)
            throw new ArgumentOutOfRangeException($"Index out of range: {index}.");

        return index == 0 ? vector.X : vector.Y;
    }

    /// <summary>Sets the element at the specified index.</summary>
    /// <param name="vector">The vector of the element to get.</param>
    /// <param name="index">The index of the element to set.</param>
    /// <param name="value">The value of the element to set.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index" /> was less than zero or greater than the number of elements.</exception>
    internal static Vector2i WithElement(Vector2i vector, int index, int value)
    {
        if ((uint)index >= Count)
            throw new ArgumentOutOfRangeException($"Index out of range: {index}.");

        var result = vector;

        if (index == 0)
            result.X = value;
        else
            result.Y = value;

        return result;
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator +(Vector2i left, Vector2i right)
    {
        return new Vector2i(left.X + right.X, left.Y + right.Y);
    }

    /// <summary>Divides the first vector by the second.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from dividing <paramref name="left" /> by <paramref name="right" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator /(Vector2i left, Vector2i right)
    {
        return new Vector2i(left.X / right.X, left.Y / right.Y);
    }

    /// <summary>Divides the specified vector by a specified scalar value.</summary>
    /// <param name="value1">The vector.</param>
    /// <param name="value2">The scalar value.</param>
    /// <returns>The result of the division.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator /(Vector2i value1, int value2)
    {
        return value1 / new Vector2i(value2);
    }

    /// <summary>Returns a new vector whose values are the product of each pair of elements in two specified vectors.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The element-wise product vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator *(Vector2i left, Vector2i right)
    {
        return new Vector2i(left.X * right.X, left.Y * right.Y);
    }

    /// <summary>Multiplies the specified vector by the specified scalar value.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator *(Vector2i left, int right)
    {
        return left * new Vector2i(right);
    }

    /// <summary>Multiplies the scalar value by the specified vector.</summary>
    /// <param name="left">The vector.</param>
    /// <param name="right">The scalar value.</param>
    /// <returns>The scaled vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator *(int left, Vector2i right)
    {
        return right * left;
    }

    /// <summary>Subtracts the second vector from the first.</summary>
    /// <param name="left">The first vector.</param>
    /// <param name="right">The second vector.</param>
    /// <returns>The vector that results from subtracting <paramref name="right" /> from <paramref name="left" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator -(Vector2i left, Vector2i right)
    {
        return new Vector2i(left.X - right.X, left.Y - right.Y);
    }

    /// <summary>Negates the specified vector.</summary>
    /// <param name="value">The vector to negate.</param>
    /// <returns>The negated vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i operator -(Vector2i value)
    {
        return Zero - value;
    }

    /// <summary>Returns a value that indicates whether each pair of elements in two specified vectors is equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(Vector2i left, Vector2i right)
    {
        return (left.X == right.X) && (left.Y == right.Y);
    }

    /// <summary>Returns a value that indicates whether two specified vectors are not equal.</summary>
    /// <param name="left">The first vector to compare.</param>
    /// <param name="right">The second vector to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, <see langword="false" />.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(Vector2i left, Vector2i right)
    {
        return !(left == right);
    }

    /// <summary>Adds two vectors together.</summary>
    /// <param name="left">The first vector to add.</param>
    /// <param name="right">The second vector to add.</param>
    /// <returns>The summed vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i Add(Vector2i left, Vector2i right)
    {
        return left + right;
    }

    /// <summary>Returns a vector whose elements are the maximum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The maximized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i Max(Vector2i value1, Vector2i value2)
    {
        return new Vector2i((value1.X > value2.X) ? value1.X : value2.X, (value1.Y > value2.Y) ? value1.Y : value2.Y);
    }

    /// <summary>Returns a vector whose elements are the minimum of each of the pairs of elements in two specified vectors.</summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The minimized vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i Min(Vector2i value1, Vector2i value2)
    {
        return new Vector2i((value1.X < value2.X) ? value1.X : value2.X, (value1.Y < value2.Y) ? value1.Y : value2.Y);
    }

    /// <summary>Restricts a vector between a minimum and a maximum value.</summary>
    /// <param name="value1">The vector to restrict.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The restricted vector.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2i Clamp(Vector2i value1, Vector2i min, Vector2i max)
    {
        return Min(Max(value1, min), max);
    }

    public bool Equals(Vector2i other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2i other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"{X}, {Y}";
    }

    public static implicit operator Vector2(Vector2i d) => new(d.X, d.Y);
}
