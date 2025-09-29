using System.Diagnostics;
using System.Numerics;

namespace Ignis.Math;

[DebuggerDisplay("Translation: {Translation} Rotation: {Rotation} Scale: {Scale}")]
public class ReadOnlyTransform
{
    /// <summary>
    /// Create a new read-only Transform which by default is at position 0, 0 with a rotation of 0.
    /// </summary>
    public ReadOnlyTransform() { }

    /// <summary>
    /// Create a new read-only Transform with the specified position and rotation.
    /// </summary>
    public ReadOnlyTransform(Vector2 translation, float rotation, Vector3 scale)
    {
        Translation = new Vector3(translation, 0);
        Rotation = Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, rotation);
        Scale = scale;
    }

    /// <summary>
    /// Create a new read-only Transform with the specified position and rotation.
    /// </summary>
    public ReadOnlyTransform(Vector3 translation, Quaternion rotation, Vector3 scale)
    {
        Translation = translation;
        Rotation = rotation;
        Scale = scale;
    }

    /// <summary>
    /// Create a new read-only Transform by decomposing the given matrix
    /// </summary>
    /// <exception cref="IgnisException">Thrown when the matrix could not be decomposed.</exception>
    public ReadOnlyTransform(Matrix4x4 matrix)
    {
        if (!Matrix4x4.Decompose(matrix, out var scale, out var rotation, out var translation))
            throw new IgnisException("Unable to decompose matrix");

        Translation = translation;
        Rotation = rotation;
        Scale = scale;
    }

    /// <summary>
    /// Copy another read-only transform
    /// </summary>
    public ReadOnlyTransform(ReadOnlyTransform transform)
    {
        Translation = transform.Translation;
        Rotation = transform.Rotation;
        Scale = transform.Scale;
    }

    /// <summary>
    /// Returns the Matrix to perform the transformation
    /// </summary>
    public Matrix4x4 Matrix
    {
        get
        {
            var matrix = Matrix4x4.CreateFromQuaternion(Rotation);
            matrix *= Matrix4x4.CreateScale(Scale);
            matrix *= Matrix4x4.CreateTranslation(Translation);
            return matrix;
        }
    }

    /// <summary>
    /// Position of the object in 2D space
    /// </summary>
    public Vector2 Translation2D => new(Translation.X, Translation.Y);

    /// <summary>
    /// Position of the object
    /// </summary>
    public Vector3 Translation { get; protected set; } = Vector3.Zero;

    /// <summary>
    /// Rotation of the object
    /// </summary>
    public Quaternion Rotation { get; protected set; } = Quaternion.Identity;

    /// <summary>
    /// Rotation of a 2D object in radians
    /// </summary>
    public float Rotation2D => Rotation.ToEulerZ();

    /// <summary>
    /// Rotation of a 2D object in degrees
    /// </summary>
    public float Rotation2DDegrees => Rotation2D.ToDegrees();

    /// <summary>
    /// Scale of the object in 2D space; should be (1, 1) by default
    /// </summary>
    public Vector2 Scale2D => new(Scale.X, Scale.Y);

    /// <summary>
    /// Scale of the object; should be (1, 1, 1) by default
    /// </summary>
    public Vector3 Scale { get; protected set; } = Vector3.One;
}
