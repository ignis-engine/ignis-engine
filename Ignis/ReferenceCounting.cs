namespace Ignis;

public interface IReferenceCounted
{
    void AddReference();
    int DecreaseReference();
}

public class ReferenceCountedBase : IReferenceCounted
{
    private int _referenceCount;

    public ReferenceCountedBase()
    {
        _referenceCount = 0;
    }

    public void AddReference()
    {
        Interlocked.Increment(ref _referenceCount);
    }

    public int DecreaseReference()
    {
        return Interlocked.Decrement(ref _referenceCount);
    }
}

public class ReferenceCounted<T> : IDisposable
    where T : IReferenceCounted, IDisposable
{
    private T? _instance;

    public ReferenceCounted(T? instance)
    {
        _instance = instance;
        _instance?.AddReference();
    }

    public void Set(T? instance)
    {
        Dispose();
        _instance = instance;
        _instance?.AddReference();
    }

    public void Dispose()
    {
        if (_instance is null)
            return;

        if (_instance.DecreaseReference() <= 0)
            _instance.Dispose();

        _instance = default;
    }

    public static implicit operator T(ReferenceCounted<T> r)
    {
        IgnisDebug.Assert(r._instance is not null, "Attempted to access a null ReferenceCounted instance.");
        return r._instance;
    }

    public static implicit operator ReferenceCounted<T>(T? instance) => new(instance);
}
