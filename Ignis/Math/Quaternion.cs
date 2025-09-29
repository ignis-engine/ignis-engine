using System.Numerics;

namespace Ignis.Math;

public static class QuaternionExtension
{
    /// <summary>
    /// Converts a Quaternion to its Euler Z-axis rotation in radians.
    /// </summary>
    /// <param name="q">The Quaternion to convert.</param>
    /// <returns>The Euler Z-axis rotation in radians.</returns>
    public static float ToEulerZ(this Quaternion q)
    {
        return (float)System.Math.Atan2(2 * (q.W * q.Z + q.X * q.Y), 1 - 2 * (q.Y * q.Y + q.Z * q.Z));
    }
}
