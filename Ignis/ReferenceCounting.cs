namespace Ignis;

public interface IReferenceCounted
{
    int AddReference();
    int DecreaseReference();
}

public class ReferenceCountedBase : IReferenceCounted
{
    private int _referenceCount;

    public ReferenceCountedBase()
    {
        _referenceCount = 0;
    }

    public int AddReference()
    {
        return Interlocked.Increment(ref _referenceCount);
    }

    public int DecreaseReference()
    {
        return Interlocked.Decrement(ref _referenceCount);
    }
}

public class ReferenceCounted<T> : ReferenceCountedBase, IDisposable
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

    public static implicit operator T?(ReferenceCounted<T> r) => r._instance;
}
