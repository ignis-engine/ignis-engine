namespace Ignis.Math;

public static class FloatExtensions
{
    /// <summary>
    /// Convert a float value representing an angle from Radians to Degrees
    /// </summary>
    /// <param name="value">An angle value in Radians</param>
    /// <returns>An angle value in Degrees</returns>
    public static float ToDegrees(this float value)
    {
        return value * (180.0f / MathF.PI);
    }

    /// <summary>
    /// Convert a float value representing an angle from Degrees to Radians
    /// </summary>
    /// <param name="value">An angle value in Degrees</param>
    /// <returns>An angle value in Radians</returns>
    public static float ToRadians(this float value)
    {
        return value * (MathF.PI / 180.0f);
    }
}
