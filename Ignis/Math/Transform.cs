using System.Diagnostics;
using System.Numerics;

namespace Ignis.Math;

/// <summary>
/// Represents a 2D transformation that can be applied to objects in a 2D space.
/// </summary>
[DebuggerDisplay("Translation: {Translation} Rotation: {Rotation} Scale: {Scale} Dirty: {IsDirty}")]
public class Transform : ReadOnlyTransform
{
    /// <summary>
    /// Called whenever the transform is changed in any way that
    /// would result the internal dirty flag to be set. Note that
    /// changing the dirty flag itself does not raise an OnChanged
    /// event.
    /// </summary>
    public EventHandler? OnChanged;

    /// <summary>
    /// Create a new Transform which by default is at position 0, 0 with a rotation of 0.
    /// </summary>
    public Transform() { }

    /// <summary>
    /// Create a new Transform with the specified position and rotation.
    /// </summary>
    public Transform(Vector2 translation, float rotation, Vector3 scale, EventHandler? onChanged = null)
        : base(translation, rotation, scale)
    {
        IsDirty = false;
        OnChanged = onChanged;
    }

    /// <summary>
    /// Create a new Transform with the specified position and rotation.
    /// </summary>
    public Transform(Vector3 translation, Quaternion rotation, Vector3 scale, EventHandler? onChanged = null)
        : base(translation, rotation, scale)
    {
        IsDirty = false;
        OnChanged = onChanged;
    }

    /// <summary>
    /// Create a new Transform by decomposing the given matrix
    /// </summary>
    /// <exception cref="IgnisException">Thrown when the matrix could not be decomposed.</exception>
    public Transform(Matrix4x4 matrix)
        : base(matrix) { }

    /// <summary>
    /// Copy another transform
    /// </summary>
    public Transform(Transform transform)
        : base(transform)
    {
        IsDirty = transform.IsDirty;
        OnChanged = transform.OnChanged;
    }

    /// <summary>
    /// Position of the object in 2D space
    /// </summary>
    public new Vector2 Translation2D
    {
        get => base.Translation2D;
        set => SetPosition(value);
    }

    /// <summary>
    /// Position of the object
    /// </summary>
    public new Vector3 Translation
    {
        get => base.Translation;
        set => SetPosition(value);
    }

    /// <summary>
    /// Rotation of the object
    /// </summary>
    public new Quaternion Rotation
    {
        get => base.Rotation;
        set => SetRotation(value);
    }

    /// <summary>
    /// Rotation of a 2D object in radians
    /// </summary>
    public new float Rotation2D
    {
        get => base.Rotation2D;
        set => SetRotation(value);
    }

    /// <summary>
    /// Rotation of a 2D object in degrees
    /// </summary>
    public new float Rotation2DDegrees
    {
        get => base.Rotation2DDegrees;
        set => SetRotationDegrees(value);
    }

    /// <summary>
    /// Scale of the object in 2D space; should be (1, 1) by default
    /// </summary>
    public new Vector2 Scale2D
    {
        get => base.Scale2D;
        set => SetScale(new Vector3(value.X, value.Y, 1));
    }

    /// <summary>
    /// Scale of the object; should be (1, 1, 1) by default
    /// </summary>
    public new Vector3 Scale
    {
        get => base.Scale;
        set => SetScale(value);
    }

    /// <summary>
    /// True if the object was modified. This can be used to
    /// detect if certain things need to be recalculated as a
    /// result. The dirty state is managed completely internally
    /// and can not be modified through the public interface by design.
    /// </summary>
    internal bool IsDirty { get; private set; }

    /// <summary>
    /// Set the position and rotation to 0
    /// Sets the dirty flag.
    /// </summary>
    public void SetIdentity()
    {
        base.Translation = Vector3.Zero;
        base.Rotation = Quaternion.Identity;
        base.Scale = Vector3.One;
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Set the position of the object in 2D space
    /// Sets the dirty flag.
    /// </summary>
    public void SetPosition(float x, float y)
    {
        SetPosition(new Vector3(x, y, 0));
    }

    /// <summary>
    /// Set the position of the object in 2D space
    /// Sets the dirty flag.
    /// </summary>
    public void SetPosition(Vector2 position)
    {
        base.Translation = new Vector3(position, 0);
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Set the position of the object in 3D space
    /// Sets the dirty flag.
    /// </summary>
    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x, y, z));
    }

    /// <summary>
    /// Set the position of the object in 3D space
    /// Sets the dirty flag.
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        base.Translation = position;
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Translate the internal position in 2D space (relative to the current position).
    /// Sets the dirty flag.
    /// </summary>
    public void Translate(float x, float y)
    {
        Translate(new Vector3(x, y, 0));
    }

    /// <summary>
    /// Translate the internal position in 2D space (relative to the current position).
    /// Sets the dirty flag.
    /// </summary>
    public void Translate(Vector2 translation)
    {
        Translate(new Vector3(translation.X, translation.Y, 0));
    }

    /// <summary>
    /// Translate the internal position in 3D space (relative to the current position).
    /// Sets the dirty flag.
    /// </summary>
    public void Translate(float x, float y, float z)
    {
        Translate(new Vector3(x, y, z));
    }

    /// <summary>
    /// Translate the internal position in 3D space (relative to the current position).
    /// Sets the dirty flag.
    /// </summary>
    public void Translate(Vector3 translation)
    {
        base.Translation += translation;
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Set the rotation of the object in 2D space.
    /// Angle must be given in radians.
    /// Sets the dirty flag.
    /// </summary>
    /// <param name="angle">An angle value in Radians</param>
    public void SetRotation(float angle)
    {
        base.Rotation = Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, angle);
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Set the rotation of the object in 2D space.
    /// Angle must be given in degrees.
    /// Sets the dirty flag.
    /// </summary>
    /// <param name="angle">An angle value in Degrees</param>
    public void SetRotationDegrees(float angle)
    {
        SetRotation(angle.ToRadians());
    }

    /// <summary>
    /// Set the rotation of the object
    /// Angle must be given in radians.
    /// Sets the dirty flag.
    /// </summary>
    public void SetRotation(Quaternion quaternion)
    {
        base.Rotation = quaternion;
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Rotate the object relative to the current rotation in 2D space
    /// Angle must be given in radians.
    /// Sets the dirty flag.
    /// </summary>
    /// <param name="angle">An angle value in Radians</param>
    public void Rotate(float angle)
    {
        Rotate(Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, angle));
    }

    /// <summary>
    /// Rotate the object relative to the current rotation in 2D space
    /// Angle must be given in degrees.
    /// Sets the dirty flag.
    /// </summary>
    /// <param name="angle">An angle value in Degrees</param>
    public void RotateDegrees(float angle)
    {
        Rotate(angle.ToRadians());
    }

    /// <summary>
    /// Rotate the object relative to the current rotation
    /// Sets the dirty flag.
    /// </summary>
    public void Rotate(Quaternion quaternion)
    {
        base.Rotation *= quaternion;
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Set the scale of the object in 2D space.
    /// Sets the dirty flag.
    /// </summary>
    public void SetScale(float x, float y)
    {
        SetScale(new Vector3(x, y, 0));
    }

    /// <summary>
    /// Set the scale of the object in 3D space.
    /// Sets the dirty flag.
    /// </summary>
    public void SetScale(float x, float y, float z)
    {
        SetScale(new Vector3(x, y, z));
    }

    /// <summary>
    /// Set the scale of the object over all axis
    /// Sets the dirty flag.
    /// </summary>
    public void SetScale(float scale)
    {
        SetScale(new Vector3(scale, scale, scale));
    }

    /// <summary>
    /// Set the scale of the object in 2D space
    /// Sets the dirty flag.
    /// </summary>
    public void SetScale(Vector2 scale)
    {
        SetScale(new Vector3(scale, 1.0f));
    }

    /// <summary>
    /// Set the scale of the object in 3D space
    /// Sets the dirty flag.
    /// </summary>
    public void SetScale(Vector3 scale)
    {
        base.Scale = scale;
        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Attempt to decompose an existing matrix back into its component parts.
    /// </summary>
    /// <exception cref="IgnisException">Thrown when the matrix could not be decomposed.</exception>
    public void DecomposeFromMatrix(Matrix4x4 matrix)
    {
        var decomposedTransform = new Transform(matrix);
        base.Rotation = decomposedTransform.Rotation;
        base.Translation = decomposedTransform.Translation;
        base.Scale = decomposedTransform.Scale;

        SetDirty();
        RaiseOnChanged();
    }

    /// <summary>
    /// Set the internal dirty flag.
    /// </summary>
    internal void SetDirty()
    {
        IsDirty = true;
    }

    /// <summary>
    /// Clear the internal dirty flag.
    /// </summary>
    internal void ClearDirty()
    {
        IsDirty = false;
    }

    private void RaiseOnChanged()
    {
        OnChanged?.Invoke(this, EventArgs.Empty);
    }
}
