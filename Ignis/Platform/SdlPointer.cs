namespace Ignis.Platform;

internal unsafe struct SdlPointer<T, TE> : IDisposable
    where T : unmanaged
    where TE : Exception, new()
{
    public T* Pointer { get; private set; }

    public bool IsNull => Pointer is null;

    public SdlPointer(T* pointer)
    {
        Pointer = pointer;

        if (Pointer is null)
            throw new TE();
    }

    public void Dispose()
    {
        Pointer = null;
    }

    public static implicit operator T*(SdlPointer<T, TE> sdlPointer) => sdlPointer.Pointer;

    public static implicit operator SdlPointer<T, TE>(T* pointer) => new(pointer);
}
